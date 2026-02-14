using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Restaurant.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        // ====================================================================
        // REGISTER
        // ====================================================================
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already registered");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                PreferredLanguage = "en",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return await GenerateAuthResponseAsync(user);
        }

        // ====================================================================
        // LOGIN
        // ====================================================================
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, dto.Password, lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    throw new InvalidOperationException("Account locked");

                throw new InvalidOperationException("Invalid email or password");
            }

            return await GenerateAuthResponseAsync(user);
        }

        // ====================================================================
        // LOGOUT
        // ====================================================================
        public async Task<bool> LogoutAsync(string userId)
        {
            return await _unitOfWork.RefreshToken.RevokeAllUserTokensAsync(userId);
        }

        // ====================================================================
        // REFRESH TOKEN
        // ====================================================================
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                throw new InvalidOperationException("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            // Revoke old token
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshToken.Update(storedToken);
            await _unitOfWork.SaveChangesAsync();

            // Generate new tokens
            return await GenerateAuthResponseAsync(user, storedToken.Token);
        }

        // ====================================================================
        // REVOKE TOKEN
        // ====================================================================
        public async Task<bool> RevokeTokenAsync(string refreshToken, string userId)
        {
            var token = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);

            if (token == null || token.UserId != userId)
                return false;

            return await _unitOfWork.RefreshToken.RevokeTokenAsync(refreshToken);
        }

        // ====================================================================
        // REVOKE ALL TOKENS
        // ====================================================================
        public async Task<bool> RevokeAllTokensAsync(string userId)
        {
            return await _unitOfWork.RefreshToken.RevokeAllUserTokensAsync(userId);
        }

        // ====================================================================
        // GET CURRENT USER
        // ====================================================================
        public async Task<UserDto> GetCurrentUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                PreferredLanguage = user.PreferredLanguage,
                Roles = roles.ToList()
            };
        }

        // ====================================================================
        // UPDATE PROFILE
        // ====================================================================
        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // ====================================================================
        // CHANGE PASSWORD
        // ====================================================================
        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            var result = await _userManager.ChangePasswordAsync(
                user, dto.CurrentPassword, dto.NewPassword
            );

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }

            // Revoke all tokens after password change
            await _unitOfWork.RefreshToken.RevokeAllUserTokensAsync(userId);

            return true;
        }

        // ====================================================================
        // UPDATE LANGUAGE
        // ====================================================================
        public async Task<bool> UpdateLanguageAsync(string userId, UpdateLanguageDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new InvalidOperationException("User not found");

            user.PreferredLanguage = dto.Language;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // ====================================================================
        // PRIVATE HELPER METHODS
        // ====================================================================

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(
            ApplicationUser user,
            string? replacedToken = null)
        {
            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["Jwt:RefreshTokenValidityInDays"] ?? "7")
                ),
                CreatedAt = DateTime.UtcNow,
                ReplacedByToken = replacedToken
            };

            await _unitOfWork.RefreshToken.Create(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:TokenValidityInHours"] ?? "2")
                ),
                RefreshTokenExpiration = refreshTokenEntity.ExpiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    PreferredLanguage = user.PreferredLanguage,
                    Roles = roles.ToList()
                }
            };
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new("PreferredLanguage", user.PreferredLanguage),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    int.Parse(_configuration["Jwt:TokenValidityInHours"] ?? "2")
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Auth;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.Interfaces;
using System.Security.Claims;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Authentication and user account management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register new customer account
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Registration successful"));
        }

        /// <summary>
        /// Login to system
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }

        /// <summary>
        /// Logout and revoke all tokens
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.LogoutAsync(userId);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Logged out successfully"));
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(ApiResponseDto<AuthResponseDto>.SuccessResponse(result, "Token refreshed"));
        }

        /// <summary>
        /// Revoke specific refresh token
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _authService.RevokeTokenAsync(dto.RefreshToken, userId);

            if (!result)
                return BadRequest(ApiResponseDto<bool>.ErrorResponse("Token not found or invalid"));

            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Token revoked"));
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _authService.GetCurrentUserAsync(userId);
            return Ok(ApiResponseDto<UserDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Profile updated"));
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.ChangePasswordAsync(userId, dto);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Password changed successfully"));
        }

        /// <summary>
        /// Update preferred language
        /// </summary>
        [HttpPut("language")]
        [Authorize]
        public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.UpdateLanguageAsync(userId, dto);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Language updated"));
        }

        /// <summary>
        /// Request password reset token (forgot password)
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);
            return Ok(ApiResponseDto<ForgotPasswordResponseDto>.SuccessResponse(result, result.Message));
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Password reset successfully"));
        }
    }
}


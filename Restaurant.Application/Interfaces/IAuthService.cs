using Restaurant.Application.DTOS.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<bool> LogoutAsync(string userId);

        // Token Management
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken, string userId);
        Task<bool> RevokeAllTokensAsync(string userId);

        // User Management
        Task<UserDto> GetCurrentUserAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<bool> UpdateLanguageAsync(string userId, UpdateLanguageDto dto);
    }
}

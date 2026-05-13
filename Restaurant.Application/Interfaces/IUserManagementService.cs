using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;

namespace Restaurant.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<ApiResponseDto<PagedResultDto<AdminUserDto>>> GetAllUsersAsync(PaginationDto pagination);
        Task<ApiResponseDto<AdminUserDto>> GetUserByIdAsync(string userId);
        Task<ApiResponseDto<AdminUserDto>> CreateUserAsync(AdminCreateUserDto dto);
        Task<ApiResponseDto<bool>> AssignRolesToUserAsync(AssignRolesDto dto);
        Task<ApiResponseDto<bool>> ToggleUserStatusAsync(string userId);
        Task<ApiResponseDto<bool>> DeleteUserAsync(string userId);
        Task<ApiResponseDto<List<RoleDto>>> GetAllRolesAsync();
    }
}


using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;

namespace Restaurant.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponseDto<AdminDashboardDto>> GetAdminDashboardAsync();
        Task<ApiResponseDto<ManagerDashboardDto>> GetManagerDashboardAsync();
        Task<ApiResponseDto<StaffDashboardDto>> GetStaffDashboardAsync();
    }
}


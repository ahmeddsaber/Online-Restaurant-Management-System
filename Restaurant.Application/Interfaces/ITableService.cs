using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface ITableService
    {
        // Admin
        Task<ApiResponseDto<PagedResultDto<AdminTableDto>>> GetAllTablesAsync(PaginationDto pagination);
        Task<ApiResponseDto<AdminTableDto>> CreateTableAsync(AdminCreateTableDto dto);
        Task<ApiResponseDto<AdminTableDto>> UpdateTableAsync(AdminUpdateTableDto dto);
        Task<ApiResponseDto<bool>> DeleteTableAsync(int id);

        // Customer
        Task<ApiResponseDto<List<AvailableTableDto>>> GetAvailableTablesForCustomerAsync();

        // Staff
        Task<ApiResponseDto<List<StaffTableDto>>> GetTablesForStaffAsync();
        Task<ApiResponseDto<bool>> UpdateTableAvailabilityAsync(UpdateTableAvailabilityDto dto);
    }
}

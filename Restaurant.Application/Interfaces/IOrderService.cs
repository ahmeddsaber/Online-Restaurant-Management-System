using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IOrderService
    {
        // Admin Functions
        Task<IEnumerable<AdminOrderDto>> SearchForAdmin(string orderName);
        Task<IEnumerable<ManagerOrderDto>> SearchForManager(string orderName);
        Task<IEnumerable<StaffOrderDto>> SearchForStaff(string orderName);
        Task<IEnumerable<CustomerOrderDto>> SearchForCustomer(string orderName);
       
        Task<IEnumerable<AdminOrderDto>> GetAllOrdersForAdminAsync();
        Task<AdminOrderDto?> GetOrderByIdForAdminAsync(int orderId);
        Task<IEnumerable<AdminOrderDto>> GetOrdersByStatusForAdminAsync(OrderStatus status);
        Task<IEnumerable<AdminOrderDto>> GetOrdersByDateRangeForAdminAsync(DateTime startDate, DateTime endDate);
        Task<bool> DeleteOrderAsync(int orderId);

        // Manager Functions
        Task<IEnumerable<ManagerOrderDto>> GetAllOrdersForManagerAsync();
        Task<ManagerOrderDto?> GetOrderByIdForManagerAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<IEnumerable<ManagerOrderDto>> GetActiveOrdersForManagerAsync();
        Task<IEnumerable<ManagerOrderDto>> GetTodayOrdersForManagerAsync();

        // Customer Functions
        Task<CustomerOrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string customerId);
        Task<CustomerOrderDto?> GetOrderByIdForCustomerAsync(int orderId, string customerId);
        Task<IEnumerable<CustomerOrderDto>> GetMyOrdersAsync(string customerId);
        Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(string customerId);
        Task<bool> CancelOrderAsync(CancelOrderDto cancelOrderDto, string customerId);
        Task<CustomerOrderDto?> GetCurrentActiveOrderAsync(string customerId);

        // Staff Functions
        Task<IEnumerable<StaffOrderDto>> GetOrdersForKitchenAsync();
        Task<bool> UpdateOrderStatusByStaffAsync(StaffUpdateOrderStatusDto dto);
        Task<StaffOrderDto?> GetOrderByIdForStaffAsync(int orderId);

        // Common Functions
        Task<bool> ValidateOrderAsync(int orderId);
        Task<string> GenerateOrderNumberAsync();
    }

}

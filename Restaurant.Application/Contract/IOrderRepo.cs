using Restaurant.Application.DTOS.Admin;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IOrderRepo : IGenaricRepository<Order>
    {
        Task<IEnumerable<Order>> SearchForAdmin(string OrderNumber);
        Task<IEnumerable<Order>> SearchForManager(string OrderNumber);
        Task<IEnumerable<Order>> SearchForStaff(string OrderNumber);
        Task<IEnumerable<Order>> SearchForCustomer(string OrderNumber);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
        Task<IEnumerable<Order>> GetActiveOrdersAsync();
        Task<IEnumerable<Order>> GetTodayOrdersAsync();
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetOrdersCountByStatusAsync(OrderStatus status);
    }
}

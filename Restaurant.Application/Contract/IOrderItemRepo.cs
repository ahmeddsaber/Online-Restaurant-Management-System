using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IOrderItemRepo : IGenaricRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId);
        Task<bool> DeleteOrderItemAsync(int orderItemId);
    }
}

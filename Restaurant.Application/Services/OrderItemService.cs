using Restaurant.Application.Contract;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
        {
            var items = await _unitOfWork.OrderItem.GetAllOrderItemsAsync();
            if (items == null || !items.Any())
            {
                throw new ArgumentException("No order items found.");
            }
            return items;
        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
        {
            if(orderItemId < 0) {
                throw new ArgumentException("Invalid order item ID.");
            }
            if((await _unitOfWork.OrderItem.GetOrderItemByIdAsync(orderItemId)) == null)
            {
                throw new ArgumentException("Order item does not exist.");
            }
            return await _unitOfWork.OrderItem.GetOrderItemByIdAsync(orderItemId);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.OrderItem.GetOrderItemsByOrderIdAsync(orderId);
        }

        public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem)
            
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));
            if (orderItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            if (orderItem.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");
            if((await _unitOfWork.MenuItem.GetById(orderItem.MenuItemId)) == null)
                throw new ArgumentException("Menu item does not exist.");
           
            await _unitOfWork.OrderItem.Create(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return orderItem;
        }

        public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));
            if (orderItem.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var existingItem = await _unitOfWork.OrderItem.GetOrderItemByIdAsync(orderItem.Id);
            if (existingItem == null)
                throw new ArgumentException("Order item does not exist.");

            if ((await _unitOfWork.MenuItem.GetById(orderItem.MenuItemId)) == null)
                throw new ArgumentException("Menu item does not exist.");

            if (existingItem.Order == null || existingItem.Order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot update order item in a non-pending order.");

            existingItem.Quantity = orderItem.Quantity;
            existingItem.UnitPrice = orderItem.UnitPrice;
            existingItem.SpecialInstructions = orderItem.SpecialInstructions;

            _unitOfWork.OrderItem.Update(existingItem);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderItemAsync(int orderItemId)
        {
            if (orderItemId <= 0)
                throw new ArgumentException("Invalid order item ID.");

            var orderItem = await _unitOfWork.OrderItem.GetOrderItemByIdAsync(orderItemId);

            if (orderItem == null)
                throw new ArgumentException("Order item does not exist.");

            if (orderItem.Order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Cannot delete order item from a non-pending order.");

            return await _unitOfWork.OrderItem.DeleteOrderItemAsync(orderItemId);
        }

    }
}


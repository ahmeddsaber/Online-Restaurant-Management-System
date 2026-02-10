using MapsterMapper;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ========================================================================
        // ADMIN FUNCTIONS
        // ========================================================================
        public Task<IEnumerable<AdminOrderDto>> SearchForAdmin(string status)
        {
            var orders = _unitOfWork.Order.SearchForAdmin(status);
            return Task.FromResult(_mapper.Map<IEnumerable<AdminOrderDto>>(orders));

        }
        public async Task<IEnumerable<AdminOrderDto>> GetAllOrdersForAdminAsync()
        {
            var orders = await _unitOfWork.Order.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<AdminOrderDto>>(orders);
        }

        public async Task<AdminOrderDto?> GetOrderByIdForAdminAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            return order == null ? null : _mapper.Map<AdminOrderDto>(order);
        }

        public async Task<IEnumerable<AdminOrderDto>> GetOrdersByStatusForAdminAsync(OrderStatus status)
        {
            var orders = await _unitOfWork.Order.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<AdminOrderDto>>(orders);
        }

        public async Task<IEnumerable<AdminOrderDto>> GetOrdersByDateRangeForAdminAsync(
            DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.Order.GetOrdersByDateRangeAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<AdminOrderDto>>(orders);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            if (order == null)
                return false;
            if (order.Id == orderId)
            {
                return false;

            }
                

            _unitOfWork.Order.Delete(orderId);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        // ========================================================================
        // MANAGER FUNCTIONS
        // ========================================================================

        public async Task<IEnumerable<ManagerOrderDto>> GetAllOrdersForManagerAsync()
        {
            var orders = await _unitOfWork.Order.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<ManagerOrderDto>>(orders);
        }

        public async Task<ManagerOrderDto?> GetOrderByIdForManagerAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            return order == null ? null : _mapper.Map<ManagerOrderDto>(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(dto.OrderId);
            if (order == null)
                return false;

            order.UpdateStatus((OrderStatus)dto.Status);
            _unitOfWork.Order.Update(order);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<ManagerOrderDto>> GetActiveOrdersForManagerAsync()
        {
            var orders = await _unitOfWork.Order.GetActiveOrdersAsync();
            return _mapper.Map<IEnumerable<ManagerOrderDto>>(orders);
        }

        public async Task<IEnumerable<ManagerOrderDto>> GetTodayOrdersForManagerAsync()
        {
            var orders = await _unitOfWork.Order.GetTodayOrdersAsync();
            return _mapper.Map<IEnumerable<ManagerOrderDto>>(orders);
        }

        // ========================================================================
        // CUSTOMER FUNCTIONS
        // ========================================================================

        public async Task<CustomerOrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, string customerId)
        {
            // Create new order
            var order = new Order
            {
                OrderNumber = await GenerateOrderNumberAsync(),
                CustomerId = customerId,
                OrderType = (OrderType)createOrderDto.OrderType,
                DeliveryAddress = createOrderDto.DeliveryAddress,
                SpecialInstructions = createOrderDto.SpecialInstructions,
                TableId = createOrderDto.TableId,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            // Add order items
            foreach (var itemDto in createOrderDto.Items)
            {
                var menuItem = await _unitOfWork.MenuItem.GetById(itemDto.MenuItemId);
                if (menuItem == null || !menuItem.IsAvailable)
                    throw new InvalidOperationException($"Menu item {itemDto.MenuItemId} is not available");

                var orderItem = new OrderItem
                {
                    MenuItemId = itemDto.MenuItemId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = menuItem.Price,
                    SpecialInstructions = itemDto.SpecialInstructions
                };

                order.OrderItems.Add(orderItem);
            }

            // Validate delivery order
            order.ValidateDeliveryOrder();

            // Calculate totals
            order.CalculateTotal();

            // Save order
           await _unitOfWork.Order.Create(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerOrderDto>(order);
        }

        public async Task<CustomerOrderDto?> GetOrderByIdForCustomerAsync(int orderId, string customerId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);

            if (order == null || order.CustomerId != customerId)
                return null;

            var dto = _mapper.Map<CustomerOrderDto>(order);
            dto.CanCancel = order.CanCancel();

            return dto;
        }

        public async Task<IEnumerable<CustomerOrderDto>> GetMyOrdersAsync(string customerId)
        {
            var orders = await _unitOfWork.Order.GetOrdersByCustomerIdAsync(customerId);
            var orderDtos = _mapper.Map<IEnumerable<CustomerOrderDto>>(orders).ToList();

            foreach (var dto in orderDtos)
            {
                var order = orders.FirstOrDefault(o => o.Id == dto.Id);
                if (order != null)
                    dto.CanCancel = order.CanCancel();
            }

            return orderDtos;
        }

        public async Task<IEnumerable<OrderHistoryDto>> GetOrderHistoryAsync(string customerId)
        {
            var orders = await _unitOfWork.Order.GetOrdersByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<OrderHistoryDto>>(orders);
        }

        public async Task<bool> CancelOrderAsync(CancelOrderDto cancelOrderDto, string customerId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(cancelOrderDto.OrderId);

            if (order == null || order.CustomerId != customerId)
                return false;

            try
            {
                order.Cancel();
                _unitOfWork.Order.Update(order);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public async Task<CustomerOrderDto?> GetCurrentActiveOrderAsync(string customerId)
        {
            var orders = await _unitOfWork.Order.GetOrdersByCustomerIdAsync(customerId);
            var activeOrder = orders.FirstOrDefault(o =>
                o.Status == OrderStatus.Pending ||
                o.Status == OrderStatus.Preparing ||
                o.Status == OrderStatus.Ready);

            return activeOrder == null ? null : _mapper.Map<CustomerOrderDto>(activeOrder);
        }

        // ========================================================================
        // STAFF FUNCTIONS
        // ========================================================================

        public async Task<IEnumerable<StaffOrderDto>> GetOrdersForKitchenAsync()
        {
            var orders = await _unitOfWork.Order.GetActiveOrdersAsync();
            return _mapper.Map<IEnumerable<StaffOrderDto>>(orders);
        }

        public async Task<bool> UpdateOrderStatusByStaffAsync(StaffUpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(dto.OrderId);
            if (order == null)
                return false;

            order.UpdateStatus((OrderStatus)dto.NewStatus);
            _unitOfWork.Order.Update(order);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<StaffOrderDto?> GetOrderByIdForStaffAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            return order == null ? null : _mapper.Map<StaffOrderDto>(order);
        }

        // ========================================================================
        // COMMON FUNCTIONS
        // ========================================================================

        public async Task<bool> ValidateOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Order.GetOrderByIdAsync(orderId);
            if (order == null)
                return false;

            order.ValidateDeliveryOrder();
            return true;
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow;
            var prefix = $"ORD-{today:yyyyMMdd}";

            var todayOrders = await _unitOfWork.Order.GetTodayOrdersAsync();
            var count = todayOrders.Count() + 1;

            return $"{prefix}-{count:D4}";
        }

        

        public Task<IEnumerable<ManagerOrderDto>> SearchForManager(string orderName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StaffOrderDto>> SearchForStaff(string orderName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CustomerOrderDto>> SearchForCustomer(string orderName)
        {
            throw new NotImplementedException();
        }
    }
}

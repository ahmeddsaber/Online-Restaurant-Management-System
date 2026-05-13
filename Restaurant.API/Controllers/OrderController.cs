using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;
using System.Security.Claims;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Order management for all roles
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // =========================================================
        // 🔴 ADMIN
        // =========================================================

        /// <summary>
        /// Get all orders (Admin)
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersAdmin([FromQuery] PaginationDto pagination)
        {
            var orders = await _orderService.GetAllOrdersForAdminAsync(pagination);
            return Ok(ApiResponseDto<PagedResultDto<Application.DTOS.Admin.AdminOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Get order by ID (Admin)
        /// </summary>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderByIdAdmin(int id)
        {
            var order = await _orderService.GetOrderByIdForAdminAsync(id);
            if (order == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Order not found"));
            return Ok(ApiResponseDto<Application.DTOS.Admin.AdminOrderDto>.SuccessResponse(order));
        }

        /// <summary>
        /// Delete order (Admin)
        /// </summary>
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound(ApiResponseDto<bool>.ErrorResponse("Order not found"));
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order deleted successfully"));
        }

        // =========================================================
        // 🟣 MANAGER
        // =========================================================

        /// <summary>
        /// Get all orders (Manager)
        /// </summary>
        [HttpGet("manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllOrdersManager([FromQuery] PaginationDto pagination)
        {
            var orders = await _orderService.GetAllOrdersForManagerAsync(pagination);
            return Ok(ApiResponseDto<PagedResultDto<ManagerOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Get active orders (Manager)
        /// </summary>
        [HttpGet("manager/active")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var orders = await _orderService.GetActiveOrdersForManagerAsync();
            return Ok(ApiResponseDto<IEnumerable<ManagerOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Get today's orders (Manager) 
        /// </summary>
        [HttpGet("manager/today")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetTodayOrders()
        {
            var orders = await _orderService.GetTodayOrdersForManagerAsync();
            return Ok(ApiResponseDto<IEnumerable<ManagerOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Update order status (Manager)
        /// </summary>
        [HttpPut("manager/status")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(dto);
            if (!result)
                return NotFound(ApiResponseDto<bool>.ErrorResponse("Order not found"));
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order status updated"));
        }

        // =========================================================
        // 🟢 CUSTOMER
        // =========================================================

        /// <summary>
        /// Create new order (Customer)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Unauthorized"));

            var order = await _orderService.CreateOrderAsync(dto, customerId);
            return Ok(ApiResponseDto<CustomerOrderDto>.SuccessResponse(order, "Order created successfully"));
        }

        /// <summary>
        /// Get my orders (Customer)
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Unauthorized"));

            var orders = await _orderService.GetMyOrdersAsync(customerId);
            return Ok(ApiResponseDto<IEnumerable<CustomerOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Get order history (Customer)
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetHistory()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Unauthorized"));

            var orders = await _orderService.GetOrderHistoryAsync(customerId);
            return Ok(ApiResponseDto<IEnumerable<OrderHistoryDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Cancel order (Customer)
        /// </summary>
        [HttpPost("cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderDto dto)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized(ApiResponseDto<object>.ErrorResponse("Unauthorized"));

            var result = await _orderService.CancelOrderAsync(dto, customerId);
            if (!result)
                return BadRequest(ApiResponseDto<bool>.ErrorResponse("Cannot cancel order"));
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order cancelled successfully"));
        }

        // =========================================================
        // 🔵 STAFF (KITCHEN)
        // =========================================================

        /// <summary>
        /// Get kitchen orders (Staff)
        /// </summary>
        [HttpGet("kitchen")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetKitchenOrders()
        {
            var orders = await _orderService.GetOrdersForKitchenAsync();
            return Ok(ApiResponseDto<IEnumerable<StaffOrderDto>>.SuccessResponse(orders));
        }

        /// <summary>
        /// Update order status (Staff)
        /// </summary>
        [HttpPut("staff/status")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateStatusByStaff([FromBody] StaffUpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusByStaffAsync(dto);
            if (!result)
                return NotFound(ApiResponseDto<bool>.ErrorResponse("Order not found"));
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order status updated"));
        }
    }
}


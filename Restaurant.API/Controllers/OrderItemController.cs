using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Manage order items
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        /// <summary>
        /// Get all order items (Admin/Manager)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllOrderItems()
        {
            var orderItems = await _orderItemService.GetAllOrderItemsAsync();
            return Ok(ApiResponseDto<IEnumerable<OrderItem>>.SuccessResponse(orderItems));
        }

        /// <summary>
        /// Get order item by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderItemById(int id)
        {
            var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
            if (orderItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Order item not found"));

            return Ok(ApiResponseDto<OrderItem>.SuccessResponse(orderItem));
        }

        /// <summary>
        /// Get order items by order ID
        /// </summary>
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);
            return Ok(ApiResponseDto<IEnumerable<OrderItem>>.SuccessResponse(orderItems));
        }

        /// <summary>
        /// Add new order item to an order
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Customer")]
        public async Task<IActionResult> AddOrderItem([FromBody] OrderItem orderItem)
        {
            var result = await _orderItemService.AddOrderItemAsync(orderItem);
            return Ok(ApiResponseDto<OrderItem>.SuccessResponse(result, "Order item added successfully"));
        }

        /// <summary>
        /// Update an existing order item
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Customer")]
        public async Task<IActionResult> UpdateOrderItem([FromBody] OrderItem orderItem)
        {
            var result = await _orderItemService.UpdateOrderItemAsync(orderItem);
            if (!result)
                return BadRequest(ApiResponseDto<bool>.ErrorResponse("Failed to update order item"));

            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order item updated successfully"));
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Manager,Customer")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var result = await _orderItemService.DeleteOrderItemAsync(id);
            if (!result)
                return BadRequest(ApiResponseDto<bool>.ErrorResponse("Failed to delete order item"));

            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Order item deleted successfully"));
        }
    }
}


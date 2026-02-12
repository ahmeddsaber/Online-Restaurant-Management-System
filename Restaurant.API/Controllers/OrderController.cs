using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;
using System.Security.Claims;

[Route("api/orders")]
[ApiController]
//[Authorize]
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
    [HttpGet("admin")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrdersAdmin()
    {
        var orders = await _orderService.GetAllOrdersForAdminAsync();
        return Ok(orders);
    }

    [HttpGet("admin/{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderByIdAdmin(int id)
    {
        var order = await _orderService.GetOrderByIdForAdminAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpDelete("admin/{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var result = await _orderService.DeleteOrderAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    // =========================================================
    // 🟣 MANAGER
    // =========================================================
    [HttpGet("manager")]
    //[Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetAllOrdersManager()
    {
        var orders = await _orderService.GetAllOrdersForManagerAsync();
        return Ok(orders);
    }

    [HttpGet("manager/active")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetActiveOrders()
    {
        var orders = await _orderService.GetActiveOrdersForManagerAsync();
        return Ok(orders);
    }

    [HttpGet("manager/today")]
    //[Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetTodayOrders()
    {
        var orders = await _orderService.GetTodayOrdersForManagerAsync();
        return Ok(orders);
    }

    [HttpPut("manager/status")]
    //[Authorize(Roles = "Manager")]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusDto dto)
    {
        var result = await _orderService.UpdateOrderStatusAsync(dto);
        if (!result) return NotFound();
        return NoContent();
    }

    // =========================================================
    // 🟢 CUSTOMER
    // =========================================================
    [HttpPost]
    //[Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(customerId))
            return Unauthorized();

        var order = await _orderService.CreateOrderAsync(dto, customerId);
        return Ok(order);
    }

    [HttpGet("my")]
    //[Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyOrders()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(customerId))
            return Unauthorized();
        var orders = await _orderService.GetMyOrdersAsync(customerId);
        return Ok(orders);
    }

    [HttpGet("history")]
    //[Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetHistory()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(customerId))
            return Unauthorized();
        var orders = await _orderService.GetOrderHistoryAsync(customerId);
        return Ok(orders);
    }

    [HttpPost("cancel")]
    //[Authorize(Roles = "Customer")]
    public async Task<IActionResult> CancelOrder(CancelOrderDto dto)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(customerId))
            return Unauthorized();
        var result = await _orderService.CancelOrderAsync(dto, customerId);

        if (!result) return BadRequest("Cannot cancel order");
        return NoContent();
    }

    // =========================================================
    // 🔵 STAFF (KITCHEN)
    // =========================================================
    [HttpGet("kitchen")]
    //[Authorize(Roles = "Staff")]
    public async Task<IActionResult> GetKitchenOrders()
    {
        var orders = await _orderService.GetOrdersForKitchenAsync();
        return Ok(orders);
    }

    [HttpPut("staff/status")]
    //[Authorize(Roles = "Staff")]
    public async Task<IActionResult> UpdateStatusByStaff(StaffUpdateOrderStatusDto dto)
    {
        var result = await _orderService.UpdateOrderStatusByStaffAsync(dto);
        if (!result) return NotFound();
        return NoContent();
    }
}

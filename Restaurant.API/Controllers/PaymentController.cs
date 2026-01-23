using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.PayMent;
using Restaurant.Application.Interfaces.Payment;
using System.Security.Claims;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Create a new payment for an order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _paymentService.CreatePaymentAsync(dto, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Confirm a payment (after successful Stripe payment)
        /// </summary>
        [HttpPost("{paymentId}/confirm")]
        public async Task<IActionResult> ConfirmPayment(int paymentId)
        {
            var result = await _paymentService.ConfirmPaymentAsync(paymentId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Refund a payment (Admin/Manager only)
        /// </summary>
        [HttpPost("{paymentId}/refund")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RefundPayment(int paymentId, [FromBody] string reason)
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, reason);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get user payment history
        /// </summary>
        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments([FromQuery] PaginationDto pagination)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _paymentService.GetUserPaymentsAsync(userId, pagination);

            return Ok(result);
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var result = await _paymentService.GetPaymentByIdAsync(paymentId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}

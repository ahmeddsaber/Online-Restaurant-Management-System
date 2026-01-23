using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.PayMent;
using Restaurant.Application.Interfaces.Payment;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripePaymentService _stripeService;

        public PaymentService(IUnitOfWork unitOfWork, IStripePaymentService stripeService)
        {
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> CreatePaymentAsync(
            CreatePaymentDto dto,
            string userId)
        {
            // 1. Get Order
            var order = await _unitOfWork.Order.GetById(dto.OrderId);
            if (order == null)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Order not found");
            }

            // 2. Verify order belongs to user
            if (order.CustomerId != userId)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Unauthorized");
            }

            // 3. Check if payment already exists
            var existingPayment = await _unitOfWork.Payment
                .GetAll()
                .FirstOrDefaultAsync(p => p.OrderId == dto.OrderId && !p.IsDeleted);

            if (existingPayment != null)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment already exists for this order");
            }

            // 4. Create Payment based on method
            Payment payment;
            string? clientSecret = null;

            switch (dto.PaymentMethod)
            {
                case (PaymentMethod)1: // Cash
                    payment = CreateCashPayment(dto, order);
                    break;

                case (PaymentMethod)3: // Stripe
                    var stripeIntent = await _stripeService.CreatePaymentIntentAsync(
                        dto.Amount,
                        dto.Currency,
                        dto.OrderId
                    );
                    payment = CreateStripePayment(dto, order, stripeIntent.PaymentIntentId);
                    clientSecret = stripeIntent.ClientSecret;
                    break;

                default:
                    return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment method not supported");
            }

            await _unitOfWork.Repository<Payment>().Create(payment);
            await _unitOfWork.SaveChangesAsync();

            var response = new PaymentResponseDto
            {
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Status = payment.Status,
                ClientSecret = clientSecret,
                CreatedAt = payment.CreatedAt
            };

            return ApiResponseDto<PaymentResponseDto>.SuccessResponse(
                response,
                "Payment created successfully"
            );
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> ConfirmPaymentAsync(int paymentId)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetById(paymentId);
            if (payment == null)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment not found");
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment already confirmed");
            }

            // Confirm based on payment method
            if (payment.PaymentMethod == PaymentMethod.Stripe && !string.IsNullOrEmpty(payment.PaymentIntentId))
            {
                var confirmed = await _stripeService.ConfirmPaymentIntentAsync(payment.PaymentIntentId);
                if (!confirmed)
                {
                    return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment confirmation failed");
                }
            }

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            // Update Order Status
            var order = await _unitOfWork.Order.GetById(payment.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Pending; // Change to Preparing or Ready
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Order.Update(order);
            }

            _unitOfWork.Repository<Payment>().Update(payment);
            await _unitOfWork.SaveChangesAsync();

            var response = new PaymentResponseDto
            {
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };

            return ApiResponseDto<PaymentResponseDto>.SuccessResponse(
                response,
                "Payment confirmed successfully"
            );
        }

        public async Task<ApiResponseDto<bool>> RefundPaymentAsync(int paymentId, string reason)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetById(paymentId);
            if (payment == null)
            {
                return ApiResponseDto<bool>.ErrorResponse("Payment not found");
            }

            if (payment.Status != PaymentStatus.Completed)
            {
                return ApiResponseDto<bool>.ErrorResponse("Only completed payments can be refunded");
            }

            // Refund via Stripe if applicable
            if (payment.PaymentMethod == PaymentMethod.Stripe && !string.IsNullOrEmpty(payment.PaymentIntentId))
            {
                var refunded = await _stripeService.RefundPaymentAsync(
                    payment.PaymentIntentId,
                    payment.Amount
                );
                if (!refunded)
                {
                    return ApiResponseDto<bool>.ErrorResponse("Refund failed");
                }
            }

            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Payment>().Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponseDto<bool>.SuccessResponse(true, "Payment refunded successfully");
        }

        public async Task<ApiResponseDto<List<PaymentHistoryDto>>> GetUserPaymentsAsync(
            string userId,
            PaginationDto pagination)
        {
            var payments = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .Where(p => p.Order.CustomerId == userId && !p.IsDeleted)
                .Include(p => p.Order)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            var result = payments.Select(p => new PaymentHistoryDto
            {
                Id = p.Id,
                TransactionId = p.TransactionId,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                OrderId = p.OrderId,
                OrderNumber = p.Order.OrderNumber
            }).ToList();

            return ApiResponseDto<List<PaymentHistoryDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponseDto<PaymentResponseDto>> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _unitOfWork.Repository<Payment>().GetById(paymentId);
            if (payment == null)
            {
                return ApiResponseDto<PaymentResponseDto>.ErrorResponse("Payment not found");
            }

            var response = new PaymentResponseDto
            {
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Status = payment.Status,
                ReceiptUrl = payment.ReceiptUrl,
                CreatedAt = payment.CreatedAt
            };

            return ApiResponseDto<PaymentResponseDto>.SuccessResponse(response);
        }

        // Helper Methods
        private Payment CreateCashPayment(CreatePaymentDto dto, Order order)
        {
            return new Payment
            {
                TransactionId = $"CASH-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}",
                PaymentMethod = PaymentMethod.Cash,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = PaymentStatus.Pending,
                OrderId = dto.OrderId
            };
        }

        private Payment CreateStripePayment(CreatePaymentDto dto, Order order, string paymentIntentId)
        {
            return new Payment
            {
                TransactionId = $"STRIPE-{DateTime.UtcNow:yyyyMMddHHmmss}",
                PaymentMethod = PaymentMethod.Stripe,
                Amount = dto.Amount,
                Currency = dto.Currency,
                Status = PaymentStatus.Processing,
                PaymentIntentId = paymentIntentId,
                OrderId = dto.OrderId
            };
        }
    }
}

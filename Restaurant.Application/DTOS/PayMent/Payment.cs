using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.PayMent
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(1, 4)]
        public PaymentMethod PaymentMethod { get; set; } // 1=Cash, 2=CreditCard, 3=Stripe, 4=PayPal

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "EGP";

        // For Stripe/Card payments
        public string? CardToken { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? ClientSecret { get; set; } // For Stripe
        public string? CheckoutUrl { get; set; } // For PayPal
        public string? ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StripePaymentIntentDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
    }

    public class PaymentHistoryDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
    }
}

using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionId { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? PaymentIntentId { get; set; } // For Stripe
        public string? ReceiptUrl { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Foreign Key
        public int OrderId { get; set; }

        // Navigation Property
        public virtual Order Order { get; set; } = null!;
    }
}



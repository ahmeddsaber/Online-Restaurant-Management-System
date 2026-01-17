
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Foreign Keys
        public string CustomerId { get; set; } = string.Empty;
        public int? TableId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser Customer { get; set; } = null!;
        public virtual Table? Table { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Business Rules
        private const decimal TaxRate = 0.085m; // 8.5%
        private const decimal HappyHourDiscount = 0.20m; // 20%
        private const decimal BulkDiscountThreshold = 100m;
        private const decimal BulkDiscount = 0.10m; // 10%

        public void CalculateTotal()
        {
            Subtotal = OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            TaxAmount = Subtotal * TaxRate;

            DiscountAmount = 0;

            // Happy Hour Discount (3 PM - 5 PM)
            if (IsHappyHour(OrderDate))
            {
                DiscountAmount += Subtotal * HappyHourDiscount;
            }

            // Bulk Discount
            if (Subtotal > BulkDiscountThreshold)
            {
                DiscountAmount += Subtotal * BulkDiscount;
            }

            Total = Subtotal + TaxAmount - DiscountAmount;
        }

        private bool IsHappyHour(DateTime dateTime)
        {
            var hour = dateTime.Hour;
            return hour >= 15 && hour < 17; // 3 PM to 5 PM
        }

        public bool CanCancel()
        {
            return Status != OrderStatus.Ready &&
                   Status != OrderStatus.Delivered &&
                   Status != OrderStatus.Cancelled;
        }

        public void Cancel()
        {
            if (!CanCancel())
            {
                throw new InvalidOperationException("Cannot cancel orders that are Ready or Delivered.");
            }
            Status = OrderStatus.Cancelled;
        }

        public void ValidateDeliveryOrder()
        {
            if (OrderType == OrderType.Delivery && string.IsNullOrWhiteSpace(DeliveryAddress))
            {
                throw new InvalidOperationException("Delivery orders must have a delivery address.");
            }
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            if (newStatus == OrderStatus.Delivered)
            {
                CompletedAt = DateTime.UtcNow;
            }
        }
    }

    
}

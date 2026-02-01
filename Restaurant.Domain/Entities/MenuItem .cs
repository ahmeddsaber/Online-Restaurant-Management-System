using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Domain.Entities
{
    public class MenuItem : BaseEntity
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int PreparationTime { get; set; } // in minutes
        public int DailyOrderCount { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual MenuCategory Category { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Business Rules
        private const int MaxDailyOrders = 50;

        public void IncrementDailyOrderCount()
        {
            DailyOrderCount++;
            if (DailyOrderCount >= MaxDailyOrders)
            {
                IsAvailable = false;
            }
        }

        public void ResetDailyOrderCount()
        {
            DailyOrderCount = 0;
            IsAvailable = true;
        }

        public bool CanOrder()
        {
            return IsAvailable && !IsDeleted && DailyOrderCount < MaxDailyOrders;
        }
    }
}

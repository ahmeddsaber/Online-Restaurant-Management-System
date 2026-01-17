using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class MenuCategory : BaseEntity
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

        // Business Logic
        public bool HasAvailableItems()
        {
            return MenuItems.Any(m => m.IsAvailable && !m.IsDeleted);
        }
    }
}

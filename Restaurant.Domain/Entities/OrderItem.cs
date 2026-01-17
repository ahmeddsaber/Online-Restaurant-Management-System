using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    // Entities/OrderItem.cs
  

    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SpecialInstructions { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }

        // Navigation Properties
        public virtual Order Order { get; set; } = null!;
        public virtual MenuItem MenuItem { get; set; } = null!;

        public decimal GetLineTotal()
        {
            return Quantity * UnitPrice;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.MenuItem
{
    public class MenuItemDto
    {
    //  public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTime { get; set; }
        public string? ImageUrl { get; set; }

        // بيانات التصنيف بدون الدخول في حلقة
        public int CategoryId { get; set; }
        public string CategoryNameEn { get; set; } = string.Empty;
        public string CategoryNameAr { get; set; } = string.Empty;
    }
}

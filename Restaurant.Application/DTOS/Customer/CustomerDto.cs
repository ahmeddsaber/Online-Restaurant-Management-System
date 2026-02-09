using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Customer
{
    public class CustomerMenuCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public List<CustomerMenuItemDto> MenuItems { get; set; } = new();
    }

    public class CustomerMenuItemDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int PreparationTime { get; set; }
        public string? ImageUrl { get; set; }
        public bool CanOrder { get; set; }
    }

     { get; set; }
// ========================================================================
    // ORDER
    // ========================================================================

    public class CreateOrderDto
    {
        [Required]
        [Range(1, 3)]
        public int OrderType { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [StringLength(500)]
        public string? DeliveryAddress
        [StringLength(1000)]
        public string? SpecialInstructions { get; set; }

        public int? TableId { get; set; }
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }
    }

    public class CustomerOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? EstimatedTime { get; set; }
        public List<CustomerOrderItemDto> Items { get; set; } = new();
        public bool CanCancel { get; set; }
    }

    public class CustomerOrderItemDto
    {
        public int Id { get; set; }
        public string ItemNameEn { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public string? SpecialInstructions { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class OrderHistoryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemsCount { get; set; }
        public string OrderType { get; set; } = string.Empty;
    }

    public class CancelOrderDto
    {
        [Required]
        public int OrderId { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }
    }

    // ========================================================================
    // PROFILE
    // ========================================================================

    public class CustomerProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string PreferredLanguage { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class UpdateCustomerProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }
    }

    // ========================================================================
    // TABLES (For Dine-In)
    // ========================================================================

    public class AvailableTableDto
    {
        public int Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public bool IsAvailable { get; set; }
    }
}
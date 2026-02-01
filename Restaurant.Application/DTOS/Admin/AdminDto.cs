using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Admin
{
    // ========================================================================
    // USER MANAGEMENT
    // ========================================================================
    
    // Admin View User
    public class AdminUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}

// Admin Create User (Manager/Staff)
public class AdminCreateUserDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one role must be selected")]
    public List<string> Roles { get; set; } = new();
}

// Assign Roles to User
public class AssignRolesDto
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one role must be selected")]
    public List<string> Roles { get; set; } = new();
}

// Role Info
public class RoleDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UsersCount { get; set; }
}

// ========================================================================
// DASHBOARD
// ========================================================================

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalStaff { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int TotalMenuItems { get; set; }
    public int TotalCategories { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<TopSellingItemDto> TopSellingItems { get; set; } = new();
    public List<RevenueByDayDto> RevenueChart { get; set; } = new();
}

public class RecentOrderDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}

public class TopSellingItemDto
{
    public int MenuItemId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public string? ImageUrl { get; set; }
}

public class RevenueByDayDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

// ========================================================================
// MENU CATEGORY MANAGEMENT
// ========================================================================

public class AdminCreateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DescriptionEn { get; set; }

    [StringLength(500)]
    public string? DescriptionAr { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdminUpdateCategoryDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(500)]
    public string? DescriptionEn { get; set; }

    [StringLength(500)]
    public string? DescriptionAr { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

public class AdminCategoryDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int MenuItemsCount { get; set; }
    public int AvailableItemsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ========================================================================
// MENU ITEM MANAGEMENT
// ========================================================================

public class AdminCreateMenuItemDto
{
    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? DescriptionEn { get; set; }

    [StringLength(1000)]
    public string? DescriptionAr { get; set; }

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 1440)]
    public int PreparationTime { get; set; }

    [Required]
    public int CategoryId { get; set; }
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
      
    public bool IsAvailable { get; set; } = true;
}

public class AdminUpdateMenuItemDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string NameAr { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? DescriptionEn { get; set; }

    [StringLength(1000)]
    public string? DescriptionAr { get; set; }

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Price { get; set; }

    [Required]
    [Range(1, 1440)]
    public int PreparationTime { get; set; }

    [Required]
    public int CategoryId { get; set; }

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        public bool IsAvailable { get; set; }
}

public class AdminMenuItemDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int PreparationTime { get; set; }
    public int DailyOrderCount { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryNameEn { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ========================================================================
// TABLE MANAGEMENT
// ========================================================================

public class AdminCreateTableDto
{
    [Required]
    [StringLength(20)]
    public string TableNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 20)]
    public int Capacity { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public bool IsAvailable { get; set; } = true;
}

public class AdminUpdateTableDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string TableNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 20)]
    public int Capacity { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    public bool IsAvailable { get; set; }
}

public class AdminTableDto
{
    public int Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
    public string? Location { get; set; }
    public int? CurrentOrderId { get; set; }
    public string? CurrentOrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ========================================================================
// ORDER MANAGEMENT
// ========================================================================

public class AdminOrderDto
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
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public int? TableId { get; set; }
    public string? TableNumber { get; set; }
    public List<AdminOrderItemDto> Items { get; set; } = new();
}

public class AdminOrderItemDto
{
    public int Id { get; set; }
    public string ItemNameEn { get; set; } = string.Empty;
    public string ItemNameAr { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string? SpecialInstructions { get; set; }
}
}
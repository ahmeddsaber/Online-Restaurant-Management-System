using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Manager
{
    // ========================================================================
    // REPORTS
    // ========================================================================
    
    public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscounts { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<DailySalesDto> DailySales { get; set; } = new();
    public List<CategorySalesDto> CategorySales { get; set; } = new();
    public List<OrderTypeSalesDto> OrderTypeSales { get; set; } = new();
}

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class CategorySalesDto
{
    public string CategoryNameEn { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int ItemsSold { get; set; }
    public decimal Percentage { get; set; }
}

public class OrderTypeSalesDto
{
    public string OrderType { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}

public class InventoryReportDto
{
    public List<MenuItemStockDto> MenuItems { get; set; } = new();
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
    public int TotalItems { get; set; }
    public int AvailableItems { get; set; }
    public int UnavailableItems { get; set; }
}

public class MenuItemStockDto
{
    public int MenuItemId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string CategoryNameEn { get; set; } = string.Empty;
    public string CategoryNameAr { get; set; } = string.Empty;
    public int DailyOrderCount { get; set; }
    public bool IsAvailable { get; set; }
    public int RemainingOrders { get; set; }
}

public class LowStockItemDto
{
    public int MenuItemId { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public int DailyOrderCount { get; set; }
    public int RemainingOrders { get; set; }
    public string AlertLevel { get; set; } = string.Empty; // Critical, Warning
}

// ========================================================================
// ORDER MANAGEMENT
// ========================================================================

public class ManagerOrderDto
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
    public DateTime OrderDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? TableNumber { get; set; }
    public List<ManagerOrderItemDto> Items { get; set; } = new();
    public bool CanCancel { get; set; }
}

public class ManagerOrderItemDto
{
    public string ItemNameEn { get; set; } = string.Empty;
    public string ItemNameAr { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class UpdateOrderStatusDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Status { get; set; }

    public string? Note { get; set; }
}

// ========================================================================
// DASHBOARD
// ========================================================================

public class ManagerDashboardDto
{
    public decimal TodayRevenue { get; set; }
    public decimal WeekRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public int TodayOrders { get; set; }
    public int PendingOrders { get; set; }
    public int PreparingOrders { get; set; }
    public List<RecentOrderSummaryDto> RecentOrders { get; set; } = new();
    public List<TopItemDto> TopItems { get; set; } = new();
}

public class RecentOrderSummaryDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
}

public class TopItemDto
{
    public string ItemNameEn { get; set; } = string.Empty;
    public string ItemNameAr { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}
}





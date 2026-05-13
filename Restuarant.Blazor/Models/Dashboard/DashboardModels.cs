namespace Restuarant.Blazor.Models.Dashboard;

public class AdminDashboardDto
{
    public int TotalOrders { get; set; }
    public int TodayOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TotalMenuItems { get; set; }
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int ActiveTables { get; set; }
    public List<RevenueChartPoint> RevenueChart { get; set; } = new();
    public List<OrderStatusCount> OrdersByStatus { get; set; } = new();
}

public class RevenueChartPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class OrderStatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

namespace Restuarant.Blazor.Models.Orders;

public class AdminOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string TableNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class UpdateOrderStatusDto
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateOrderDto
{
    public int TableId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string? Notes { get; set; }
}

public class CreateOrderItemDto
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}

public class CustomerOrderDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.DTOS.Staff
{
    public class StaffOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TableNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int EstimatedTime { get; set; } // Total preparation time
        public List<StaffOrderItemDto> Items { get; set; } = new();
        public string? SpecialInstructions { get; set; }
    }

    public class StaffOrderItemDto
    {
        public string ItemNameEn { get; set; } = string.Empty;
        public string ItemNameAr { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PreparationTime { get; set; }
        public string? SpecialInstructions { get; set; }
        public bool IsReady { get; set; }
    }

    public class StaffUpdateOrderStatusDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(2, 3)] // Only Preparing(2) or Ready(3)
        public int NewStatus { get; set; }
    }

    // ========================================================================
    // TABLE MANAGEMENT
    // ========================================================================

    public class StaffTableDto
    {
        public int Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public string? Location { get; set; }
        public int? CurrentOrderId { get; set; }
        public string? CurrentOrderNumber { get; set; }
        public string? CurrentOrderStatus { get; set; }
    }

    public class UpdateTableAvailabilityDto
    {
        [Required]
        public int TableId { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
    }

    // ========================================================================
    // DASHBOARD
    // ========================================================================

    public class StaffDashboardDto
    {
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int ReadyOrders { get; set; }
        public int AvailableTables { get; set; }
        public int OccupiedTables { get; set; }
        public List<StaffOrderSummaryDto> ActiveOrders { get; set; } = new();
    }

    public class StaffOrderSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TableNumber { get; set; }
        public int ItemsCount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}


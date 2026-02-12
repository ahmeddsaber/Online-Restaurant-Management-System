using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Exceptions
{
    public static class OrderExtensions
    {
        public static int CalculateEstimatedTime(this Order order)
        {
            if (order.OrderItems == null || !order.OrderItems.Any())
                return 0;

            // Assume average preparation time per item
            return order.OrderItems.Sum(oi => oi.Quantity * 5); // 5 minutes per item
        }

        public static bool IsDeliveryOrder(this Order order)
        {
            return order.OrderType == OrderType.Delivery;
        }

        public static bool IsDineInOrder(this Order order)
        {
            return order.OrderType == OrderType.DineIn;
        }

        public static bool IsTakeawayOrder(this Order order)
        {
            return order.OrderType == OrderType.Takeaway;
        }
    }
}

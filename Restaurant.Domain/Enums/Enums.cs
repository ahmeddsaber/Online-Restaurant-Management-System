using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Enums
{
    public enum OrderType
    {
        DineIn = 1,
        Takeaway = 2,
        Delivery = 3
    }
    public enum OrderStatus
    {
        Unknown = 0,
        Pending = 1,
        Preparing = 2,
        Ready = 3,
        Delivered = 4,
        Cancelled = 5
    }
}

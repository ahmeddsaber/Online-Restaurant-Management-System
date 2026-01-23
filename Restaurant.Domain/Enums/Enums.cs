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
    public enum PaymentMethod
    {
        Cash = 1,
        CreditCard = 2,
        Stripe = 3,
        PayPal = 4
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        Refunded = 5
    }
}

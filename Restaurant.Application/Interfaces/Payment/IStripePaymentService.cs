using Restaurant.Application.DTOS.PayMent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces.Payment
{
    public interface IStripePaymentService
    {
        Task<StripePaymentIntentDto> CreatePaymentIntentAsync(decimal amount, string currency, int orderId);
        Task<bool> ConfirmPaymentIntentAsync(string paymentIntentId);
        Task<bool> RefundPaymentAsync(string paymentIntentId, decimal amount);
    }   
}

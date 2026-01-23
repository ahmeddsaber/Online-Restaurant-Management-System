using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.PayMent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<ApiResponseDto<PaymentResponseDto>> CreatePaymentAsync(CreatePaymentDto dto, string userId);
        Task<ApiResponseDto<PaymentResponseDto>> ConfirmPaymentAsync(int paymentId);
        Task<ApiResponseDto<bool>> RefundPaymentAsync(int paymentId, string reason);
        Task<ApiResponseDto<List<PaymentHistoryDto>>> GetUserPaymentsAsync(string userId, PaginationDto pagination);
        Task<ApiResponseDto<PaymentResponseDto>> GetPaymentByIdAsync(int paymentId);
    }
}

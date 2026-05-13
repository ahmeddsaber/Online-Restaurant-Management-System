using Restuarant.Blazor.Models.Common;
using Restuarant.Blazor.Models.Orders;
using System.Net.Http.Json;
using System.Text.Json;

namespace Restuarant.Blazor.Services;

public interface IOrderApiService
{
    Task<PagedResult<AdminOrderDto>?> GetAllOrdersAdminAsync(int page = 1, int pageSize = 10);
    Task<bool> DeleteOrderAsync(int id);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
}

public class OrderApiService(HttpClient http) : IOrderApiService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<PagedResult<AdminOrderDto>?> GetAllOrdersAdminAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await http.GetAsync($"api/orders/admin?PageNumber={page}&PageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<AdminOrderDto>>>(json, _jsonOptions);
            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            var response = await http.DeleteAsync($"api/orders/admin/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            var dto = new UpdateOrderStatusDto { OrderId = orderId, Status = status };
            var response = await http.PutAsJsonAsync("api/orders/manager/status", dto);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}

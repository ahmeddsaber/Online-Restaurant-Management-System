using Restuarant.Blazor.Models.Common;
using Restuarant.Blazor.Models.Dashboard;
using System.Text.Json;

namespace Restuarant.Blazor.Services;

public interface IDashboardApiService
{
    Task<AdminDashboardDto?> GetAdminDashboardAsync();
}

public class DashboardApiService(HttpClient http) : IDashboardApiService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<AdminDashboardDto?> GetAdminDashboardAsync()
    {
        try
        {
            var response = await http.GetAsync("api/Dashboard/admin");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<AdminDashboardDto>>(json, _jsonOptions);
            return result?.Data;
        }
        catch { return null; }
    }
}

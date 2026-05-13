using Restuarant.Blazor.Models.Common;
using Restuarant.Blazor.Models.Users;
using System.Net.Http.Json;
using System.Text.Json;

namespace Restuarant.Blazor.Services;

public interface IUserApiService
{
    Task<PagedResult<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 10);
    Task<bool> CreateUserAsync(CreateUserRequest request);
    Task<bool> AssignRolesAsync(AssignRolesRequest request);
    Task<bool> ToggleUserStatusAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);
    Task<List<string>> GetAllRolesAsync();
}

public class UserApiService(HttpClient http) : IUserApiService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<PagedResult<UserDto>?> GetAllUsersAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await http.GetAsync($"api/UserManagement?PageNumber={page}&PageSize={pageSize}");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<UserDto>>>(json, _jsonOptions);
            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/UserManagement", request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> AssignRolesAsync(AssignRolesRequest request)
    {
        try
        {
            var response = await http.PutAsJsonAsync("api/UserManagement/assign-roles", request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ToggleUserStatusAsync(string userId)
    {
        try
        {
            var response = await http.PutAsync($"api/UserManagement/{userId}/toggle-status", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            var response = await http.DeleteAsync($"api/UserManagement/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        try
        {
            var response = await http.GetAsync("api/UserManagement/roles");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<string>>>(json, _jsonOptions);
            return result?.Data ?? new List<string>();
        }
        catch { return new List<string>(); }
    }
}

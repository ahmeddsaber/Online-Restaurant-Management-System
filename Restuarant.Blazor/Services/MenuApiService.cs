using Restuarant.Blazor.Models.Common;
using Restuarant.Blazor.Models.Menu;
using System.Net.Http.Json;
using System.Text.Json;

namespace Restuarant.Blazor.Services;

public interface IMenuApiService
{
    Task<List<MenuItemDto>> GetAllMenuItemsAsync();
    Task<PagedResult<MenuItemDto>?> GetPaginatedMenuItemsAsync(int page, int pageSize, string? search = null);
    Task<List<MenuCategoryDto>> GetAllCategoriesAsync();
    Task<bool> DeleteMenuItemAsync(int id);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> ToggleCategoryStatusAsync(int id);
}

public class MenuApiService(HttpClient http) : IMenuApiService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<MenuItemDto>> GetAllMenuItemsAsync()
    {
        try
        {
            var response = await http.GetAsync("api/v1/MenuItem/all");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<MenuItemDto>>>(json, _jsonOptions);
            return result?.Data ?? new List<MenuItemDto>();
        }
        catch { return new List<MenuItemDto>(); }
    }

    public async Task<PagedResult<MenuItemDto>?> GetPaginatedMenuItemsAsync(int page, int pageSize, string? search = null)
    {
        try
        {
            var url = $"api/v1/MenuItem/paginated?PageNumber={page}&PageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={search}";

            var response = await http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<MenuItemDto>>>(json, _jsonOptions);
            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<List<MenuCategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            var response = await http.GetAsync("api/v1/MenuCategory/all");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<List<MenuCategoryDto>>>(json, _jsonOptions);
            return result?.Data ?? new List<MenuCategoryDto>();
        }
        catch { return new List<MenuCategoryDto>(); }
    }

    public async Task<bool> DeleteMenuItemAsync(int id)
    {
        try
        {
            var response = await http.DeleteAsync($"api/v1/MenuItem/DeleteMenuItem/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var response = await http.DeleteAsync($"api/v1/MenuCategory/DeleteMenuCategory/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public Task<bool> ToggleCategoryStatusAsync(int id)
    {
        return Task.FromResult(true); // Placeholder
    }
}

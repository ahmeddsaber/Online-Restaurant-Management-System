using Restuarant.Blazor.Models.Auth;
using Restuarant.Blazor.Models.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace Restuarant.Blazor.Services;

public interface IAuthApiService
{
    Task<ApiResponse<AuthResponse>?> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>?> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<ApiResponse<UserInfo>?> GetCurrentUserAsync();
}

public class AuthApiService(HttpClient http, ILocalStorageService localStorage,
    Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authStateProvider) : IAuthApiService
{
    private readonly Restuarant.Blazor.Auth.JwtAuthStateProvider _authStateProvider = (Restuarant.Blazor.Auth.JwtAuthStateProvider)authStateProvider;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApiResponse<AuthResponse>?> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/Auth/login", request);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(json, _jsonOptions);

        if (result?.Success == true && result.Data != null)
        {
            await localStorage.SetItemAsync("jwtToken", result.Data.Token);
            await localStorage.SetItemAsync("refreshToken", result.Data.RefreshToken);
            _authStateProvider.NotifyUserAuthentication(result.Data.Token);
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Data.Token);
        }

        return result;
    }

    public async Task<ApiResponse<AuthResponse>?> RegisterAsync(RegisterRequest request)
    {
        var response = await http.PostAsJsonAsync("api/Auth/register", request);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<AuthResponse>>(json, _jsonOptions);
    }

    public async Task LogoutAsync()
    {
        try { await http.PostAsync("api/Auth/logout", null); } catch { }
        await localStorage.RemoveItemAsync("jwtToken");
        await localStorage.RemoveItemAsync("refreshToken");
        http.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.NotifyUserLogout();
    }

    public async Task<ApiResponse<UserInfo>?> GetCurrentUserAsync()
    {
        var response = await http.GetAsync("api/Auth/me");
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<UserInfo>>(json, _jsonOptions);
    }
}

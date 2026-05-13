using Microsoft.JSInterop;

namespace Restuarant.Blazor.Services;

public interface ILocalStorageService
{
    ValueTask<string?> GetItemAsync(string key);
    ValueTask SetItemAsync(string key, string value);
    ValueTask RemoveItemAsync(string key);
}

public class LocalStorageService(IJSRuntime jsRuntime) : ILocalStorageService
{
    public ValueTask<string?> GetItemAsync(string key)
    {
        return jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
    }

    public ValueTask SetItemAsync(string key, string value)
    {
        return jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public ValueTask RemoveItemAsync(string key)
    {
        return jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}

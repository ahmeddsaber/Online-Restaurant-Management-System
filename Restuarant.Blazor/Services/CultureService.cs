using Microsoft.JSInterop;

namespace Restuarant.Blazor.Services;

public interface ICultureService
{
    string CurrentCulture { get; }
    bool IsRtl { get; }
    Task SetCultureAsync(string culture);
    Task<string> GetCultureAsync();
    event Action? OnCultureChanged;
}

public class CultureService(ILocalStorageService localStorage) : ICultureService
{
    private string _currentCulture = "en";

    public event Action? OnCultureChanged;

    public string CurrentCulture => _currentCulture;
    public bool IsRtl => _currentCulture == "ar";

    public async Task SetCultureAsync(string culture)
    {
        _currentCulture = culture;
        await localStorage.SetItemAsync("culture", culture);
        OnCultureChanged?.Invoke();
    }

    public async Task<string> GetCultureAsync()
    {
        var saved = await localStorage.GetItemAsync("culture");
        _currentCulture = saved ?? "en";
        return _currentCulture;
    }
}

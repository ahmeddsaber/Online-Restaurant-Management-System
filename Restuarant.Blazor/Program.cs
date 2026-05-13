using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Restuarant.Blazor;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ─── HTTP Client ───────────────────────────────────────────────────────────
//builder.Services.AddScoped(sp =>
//    new HttpClient { BaseAddress = new Uri("https://localhost:7112") });

builder.Services.AddScoped<HttpClient>(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri("https://abosalahrestuarant.runasp.net")
    };
});
// ─── Auth ──────────────────────────────────────────────────────────────────
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider,
    Restuarant.Blazor.Auth.JwtAuthStateProvider>();

// ─── Infrastructure Services ───────────────────────────────────────────────
builder.Services.AddScoped<Restuarant.Blazor.Services.ILocalStorageService,
    Restuarant.Blazor.Services.LocalStorageService>();
builder.Services.AddScoped<Restuarant.Blazor.Services.ICultureService,
    Restuarant.Blazor.Services.CultureService>();

// ─── API Services ──────────────────────────────────────────────────────────
builder.Services.AddScoped<Restuarant.Blazor.Services.IAuthApiService,
    Restuarant.Blazor.Services.AuthApiService>();
builder.Services.AddScoped<Restuarant.Blazor.Services.IMenuApiService,
    Restuarant.Blazor.Services.MenuApiService>();
builder.Services.AddScoped<Restuarant.Blazor.Services.IOrderApiService,
    Restuarant.Blazor.Services.OrderApiService>();
builder.Services.AddScoped<Restuarant.Blazor.Services.IUserApiService,
    Restuarant.Blazor.Services.UserApiService>();
builder.Services.AddScoped<Restuarant.Blazor.Services.IDashboardApiService,
    Restuarant.Blazor.Services.DashboardApiService>();

await builder.Build().RunAsync();

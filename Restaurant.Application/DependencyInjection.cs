using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Interfaces.Payment;
using Restaurant.Application.Services;

namespace Restaurant.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            services.AddScoped<IMenuItemService, MenuItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IStripePaymentService, StripePaymentService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IImageService, ImageService>();

            return services;
        }
    }
}

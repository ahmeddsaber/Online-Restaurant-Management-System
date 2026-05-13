using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Contract;
using Restaurant.Infrastructure.DbContext;
using Restaurant.Infrastructure.Repository;

namespace Restaurant.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("OnlineRestaurantDB"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("Restaurant.Infrastructure");
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        }));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IMenuCategoryRepo, MenuCategoryRepository>();
            services.AddScoped<IMenuItemRepo, MenuItemRepository>();
            services.AddScoped<IOrderRepo, OrderRepository>();
            services.AddScoped<IOrderItemRepo, OrderItemRepository>();
            services.AddScoped<ITableRepo, TableRepository>();
            services.AddScoped<Application.Contract.payment.IPaymentRepo, PaymentRepository>();
            services.AddScoped<IRefreshTokenRepo, RefreshTokenRepository>();
            services.AddScoped<IUnitOfWork, Restaurant.Infrastructure.UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}

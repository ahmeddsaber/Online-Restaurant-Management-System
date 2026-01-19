using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using Restaurant.Infrastructure.Repository;
using Restaurant.Infrastructure.UnitOfWork;
using Restaurant.Infrastructure.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace Restaurant.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string RepacementDataFromFrondEnd = "";
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(RepacementDataFromFrondEnd, builder=>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Database Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlServer(
         builder.Configuration.GetConnectionString("OnlineRestaurantDB"),
         sqlOptions => sqlOptions.MigrationsAssembly("Restaurant.Infrastructure")
     ));


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Identity Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireConfirmedEmail");
                options.Password.RequireDigit = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireDigit");
                options.Password.RequiredLength = builder.Configuration.GetValue<int>("PasswordRequirements:MinimumLength");
                options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireSpecialCharacter");
                options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireUppercase");
                options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireLowercase");
                options.User.RequireUniqueEmail = builder.Configuration.GetValue<bool>("PasswordRequirements:RequireUniqueEmail");
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Dependency Injection
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddScoped(typeof(IGenaricRepository<>), typeof(GenaricRepository<>));
            builder.Services.AddScoped<IMenuCategoryRepo, MenuCategoryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            builder.Services.AddScoped<IMenuItemRepo, MenuItemRepository>();
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();
            builder.Services.AddScoped<IOrderRepo, OrderRepository>();
            builder.Services.AddScoped<IOrderService,OrderService>();
            builder.Services.AddScoped<IOrderItemRepo, OrderItemRepository>();
            builder.Services.AddScoped<IOrderItemService,OrderItemService>();




            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Swagger / OpenAPI Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
             
                    Title = "Restaurant Management API",
                    Version = "v2",
                    Description = "API with JWT Authentication and Multilingual Support",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Ahmed Saber",
                        Email = "ahmedsaberkh7@gmail.com"
                    }
                    ,
                    TermsOfService = new Uri("https://microsoft.com/terms")
                    });

                // JWT Bearer Configuration for Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Restaurant.API.xml"));
                // In the SwaggerGen configuration section, ensure the following line is present after adding the using directive above:
                options.EnableAnnotations();

            });

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Build Application
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var app = builder.Build();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Seed Database
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Apply pending migrations
                    await context.Database.MigrateAsync();

                    // Seed initial data
                    await DbInitializer.SeedAsync(context, userManager, roleManager);

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("✅ Database seeded successfully!");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "❌ An error occurred while seeding the database.");
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Configure Middleware Pipeline
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Management API v1");
                    c.RoutePrefix = "swagger";  // Make Swagger the default page
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(RepacementDataFromFrondEnd);

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}

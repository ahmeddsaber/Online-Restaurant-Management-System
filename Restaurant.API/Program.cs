using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Contract;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.DbContext;
using Restaurant.Infrastructure.Repository;
using Restaurant.Infrastructure.UnitOfWork;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection; // أضف هذا
using System.Text;

namespace Restaurant.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string AllowAllCors = "AllowAll";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            builder.Services.AddMemoryCache();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Mapster Configuration ⭐⭐⭐ أضف هذا القسم
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
            builder.Services.AddSingleton(typeAdapterConfig);
            builder.Services.AddScoped<IMapper, ServiceMapper>();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // CORS Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AllowAllCors, builder =>
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
            // Authentication Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

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
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderItemRepo, OrderItemRepository>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<ITableRepo, TableRepository>();
            builder.Services.AddScoped<ITableService, TableService>();

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Swagger / OpenAPI Configuration
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Restaurant Management API",
                    Version = "v1",
                    Description = "API with JWT Authentication and Multilingual Support",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Ahmed Saber",
                        Email = "ahmedsaberkh7@gmail.com"
                    },
                    TermsOfService = new Uri("https://microsoft.com/terms")
                });

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

                    await context.Database.MigrateAsync();
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
                    c.RoutePrefix = "swagger";
                });
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors(AllowAllCors);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}
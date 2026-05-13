using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using System.Text;
using System.Threading.RateLimiting;
using HealthChecks.UI.Client;

namespace Restaurant.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebApiConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Controllers
            services.AddControllers(options =>
            {
                options.Filters.Add<Filters.ValidationFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // FluentValidation
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(typeof(Application.DependencyInjection).Assembly);

            // Core Services
            services.AddEndpointsApiExplorer();
            //services.AddSignalR();
            services.AddMemoryCache();

            // Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(Application.DependencyInjection).Assembly);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins(
                               "https://localhost:7013",
                               "http://localhost:5219"
                           )
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });


            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant Management API",
                    Version = "v1",
                    Description = "Restaurant System API with JWT, Roles, and Versioning",
                    Contact = new OpenApiContact
                    {
                        Name = "Ahmed Saber",
                        Email = "ahmedsaberkh7@gmail.com"
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = configuration.GetValue<bool>("PasswordRequirements:RequireDigit");
                options.Password.RequiredLength = configuration.GetValue<int>("PasswordRequirements:MinimumLength");
                options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("PasswordRequirements:RequireSpecialCharacter");
                options.Password.RequireUppercase = configuration.GetValue<bool>("PasswordRequirements:RequireUppercase");
                options.Password.RequireLowercase = configuration.GetValue<bool>("PasswordRequirements:RequireLowercase");
                options.User.RequireUniqueEmail = configuration.GetValue<bool>("PasswordRequirements:RequireUniqueEmail");
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Authentication (JWT)
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // API Versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Health Checks
            services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("OnlineRestaurantDB")!)
                .AddDbContextCheck<ApplicationDbContext>();

            // Rate Limiting
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("GlobalLimit", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                });

                options.AddFixedWindowLimiter("AuthLimit", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                });
            });

            return services;
        }

        public static IApplicationBuilder UseWebConfig(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                });
            }

            app.UseCors("AllowAll");

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }
    }
}
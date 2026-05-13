using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Extensions;
using Restaurant.API.Middleware;
using Restaurant.Application;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure;
using Restaurant.Infrastructure.Data;
using Restaurant.Infrastructure.DbContext;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add Clean Architecture Layers
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddWebApiConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseExceptionHandling(); // Global Exception Handling Middleware
//app.UseStaticFiles();
//app.UseWebConfig(app.Environment);

//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
//    });
//}
//app.UseHttpsRedirection();

//app.MapControllers();
//app.MapGet("/", () => Results.Redirect("/swagger/index.html"));
app.UseExceptionHandling();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
    {
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
    c.RoutePrefix = "swagger";
});
}
app.MapGet("/", () => Results.Redirect("/swagger"));

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context, userManager, roleManager);
        
        logger.LogInformation("✅ Database seeded successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while seeding the database.");
    }
}

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


using CareerHub.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SQL Server - Entity Framework Core
builder.Services.AddDbContext<CareerHubContext>();

// Swagger API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// API Controllers
app.MapControllers();

// MVC Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapGet("/debug/routes", (
    Microsoft.AspNetCore.Routing.EndpointDataSource endpoints) =>
{
    return Results.Json(
        endpoints.Endpoints
            .OfType<Microsoft.AspNetCore.Routing.RouteEndpoint>()
            .Select(e => new
            {
                Route = e.RoutePattern.RawText,
                Name = e.DisplayName
            })
            .ToList()
    );
});
app.Run();

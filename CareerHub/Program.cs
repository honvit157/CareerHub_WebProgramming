
using CareerHub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
// Đăng ký kết nối CSDL cho CareerHubContext
builder.Services.AddDbContext<CareerHubContext>(options =>
    options.UseSqlServer("Data Source=DAVIHOAN\\SQLEXPRESS;Initial Catalog=CareerHub;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Chưa đăng nhập mà đòi vào trang cấm -> Bị đá về trang Login
        options.AccessDeniedPath = "/Account/Denied"; // Đã đăng nhập nhưng SAI VAI TRÒ -> Bị chuyển sang trang Báo lỗi cấm truy cập
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cookie sống trong 8 tiếng
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

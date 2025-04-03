using LAB03.Models;
using LAB03.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";  // Sử dụng cookie để quản lý phiên
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;  // Xác thực Google
})
.AddGoogle(options =>
{
    options.ClientId = "516501066913-j84nejbkinlg7ts9hp5kvmcqoqs7o4uo.apps.googleusercontent.com";  // Thêm Client ID của bạn ở đây
    options.ClientSecret = "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";  // Thêm Client Secret của bạn ở đây
});

builder.Services.AddRazorPages();

// Đăng ký các repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

// Thêm dịch vụ MVC


builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LoginPath = $"/Identity/Account/Logout";
    option.LoginPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();
var app = builder.Build();
// Đặt trước UseRouting
app.UseSession();




// Cấu hình pipeline xử lý HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();  // Đảm bảo sử dụng xác thực
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "admin/{controller=Home}/{action=Index}/{id?}");

    // Đảm bảo các route khác cho "product" và "account" cũng được cấu hình tương tự
    endpoints.MapControllerRoute(
        name: "product",
        pattern: "product/{controller=Product}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
    name: "Order",
    pattern: "order/{controller=OrderManagement}/{action=Index}/{id?}");


    endpoints.MapControllerRoute(
    name: "admin",
    pattern: "OrderManagement/{controller=OrderManagement}/{action=Index}/{id?}");

});


app.MapRazorPages();

app.Run();

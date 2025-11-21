// Program.cs - replace the entire file contents with this

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using CMCS_Web.Data;
using CMCS_Web.Services;
using CMCS_Web.Models;
using CMCS_Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ---------------- Configuration & Connection ----------------
var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? "Data Source=cmcs.db";

// ---------------- Services ----------------
builder.Services.AddControllersWithViews();

// Register EF DbContext (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connString));

// Register application services (ensure these interfaces/classes exist)
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IUserService, UserService>();

// Password hasher for user password handling
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// SignalR
builder.Services.AddSignalR();

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

// File upload limit (6 MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 6 * 1024 * 1024;
});

var app = builder.Build();

// Apply pending migrations (auto)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("Program");
        logger?.LogError(ex, "Database migrate error.");
    }
}

// ---------------- HTTP pipeline ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Claims}/{action=Create}/{id?}");

// Map the SignalR hub (we just added ClaimHub.cs)
app.MapHub<ClaimHub>("/claimHub");

app.Run();

using CourseSeller.Core.Services.Interfaces;
using CourseSeller.Core.Services;
using CourseSeller.DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var services = builder.Services;
var conf = builder.Configuration;
var env = builder.Environment;


#region Authentication

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = "/Account/Login/";
    options.LogoutPath = "/Account/Logout/";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});

#endregion


#region DB Context

services.AddDbContext<MssqlContext>(options =>
{
    options.UseSqlServer(conf.GetConnectionString("MssqlConnection"));
});

#endregion


#region IoC

services.AddTransient<IAccountService, AccountService>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!env.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

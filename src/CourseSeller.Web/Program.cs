using CourseSeller.Core.Services.Interfaces;
using CourseSeller.Core.Services;
using CourseSeller.DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Hangfire;
using Hangfire.SqlServer;
using CourseSeller.Core.Convertors;
using CourseSeller.Core.Senders;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var conf = builder.Configuration;
var env = builder.Environment;


#region HangFire

services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(conf.GetConnectionString("MssqlConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add the processing server as IHostedService
services.AddHangfireServer();

#endregion


// Add services to the container.
builder.Services.AddControllersWithViews();


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

services.AddTransient<IViewRenderService, RenderViewToString>();
services.AddTransient<ISendEmail, SendEmail>();
services.AddTransient<IAccountService, AccountService>();
services.AddTransient<IUserPanelService, UserPanelService>();
services.AddTransient<IAdminService, AdminService>();
services.AddTransient<IRoleService, RoleService>();
services.AddTransient<IPermissionService, PermissionService>();

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

// Hangfire dashboard
app.UseHangfireDashboard();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();

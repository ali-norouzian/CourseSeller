using CourseSeller.Core.Services.Interfaces;
using CourseSeller.Core.Services;
using CourseSeller.DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Hangfire;
using Hangfire.SqlServer;
using CourseSeller.Core.Convertors;
using CourseSeller.Core.Security;
using CourseSeller.Core.Senders;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

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

// File uploading limits.
services.Configure<FormOptions>(option =>
{
    option.MultipartBodyLengthLimit = 1 * 1024 * 1024 * 1024; // 1 GB
});


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
services.AddTransient<IImageUtils, ImageUtils>();
services.AddTransient<IPasswordHelper, PasswordHelper>();
services.AddTransient<IAccountService, AccountService>();
services.AddTransient<IUserPanelService, UserPanelService>();
services.AddTransient<IAdminService, AdminService>();
services.AddTransient<IRoleService, RoleService>();
services.AddTransient<IPermissionService, PermissionService>();
services.AddTransient<ICourseService, CourseService>();
services.AddTransient<IOrderService, OrderService>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!env.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        // Add logic to limit access to files
        // For example, you can forbid access to specific file types
        if (Path.GetExtension(context.File.Name) == ".mp4")
        {
            context.Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Context.Response.ContentLength = 0;
            context.Context.Response.Body = Stream.Null;
        }
    }
});

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

using CourseSeller.DataLayer.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var services = builder.Services;
var conf = builder.Configuration;
var env = builder.Environment;


#region DB Context

services.AddDbContext<MssqlContext>(options =>
{
    options.UseSqlServer(conf.GetConnectionString("MssqlConnection"));
});

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!env.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.DataBaseContext;

namespace Voltage;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<VoltageDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:sqlConn"]));
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                        .AddEntityFrameworkStores<VoltageDbContext>()
                        .AddDefaultTokenProviders();

        builder.Services.AddAuthentication();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "ForeAdminarea",
                areaName: "Admin",
                pattern: "foradmin/{controller=Admin}/{action=AdminPage}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=MainPage}/{action=Index}/{id?}");
        });

        app.Run();
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;

namespace Voltage;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<VoltageDbContext>(_ => _.UseSqlServer(builder.Configuration["ConnectionStrings:sqlConn"]));
        builder.Services.AddIdentity<User, IdentityRole>()
                        .AddEntityFrameworkStores<VoltageDbContext>()
                        .AddDefaultTokenProviders();

        //builder.Services.AddAuthentication();

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

        app.MapControllerRoute(
            name: "UserArea",
            pattern: "user/{controller=VoltageUser}/{action=Index}/{id?}",
            defaults: new { area = "user" }
            );

        app.MapControllerRoute(
            name: "adminArea",
            pattern: "foradmin/{controller=VoltageAdmin}/{action=Index}/{id?}",
            defaults: new { area = "admin" }
            );

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}"
            );

        app.Run();
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Helper.Validations;

namespace Voltage;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<VoltageDbContext>(_ => _.UseSqlServer(builder.Configuration["ConnectionStrings:sqlConn"]));
        builder.Services.AddIdentity<User, IdentityRole>(_ =>
        {
            _.Password.RequiredLength = 5;
            _.Password.RequireNonAlphanumeric = false;
            _.Password.RequireLowercase = true;
            _.Password.RequireUppercase = true;
            _.Password.RequireDigit = true;

            _.User.RequireUniqueEmail = true;
            _.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

            //_.SignIn.RequireConfirmedEmail = true;
        }).AddPasswordValidator<CustomIdentityValidation>()
          .AddUserValidator<CustomUserValidation>()
          .AddErrorDescriber<CustomIdentityErrorDescriber>()
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

        app.UseAuthentication();
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
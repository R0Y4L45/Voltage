using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voltage.Business.Services.Concrete;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Entities.Models;
using Voltage.Helper.Validations;
using Voltage.Services.HostedService;
using Voltage.Hubs;

namespace Voltage;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<VoltageDbContext>(_ => _.UseSqlServer(builder.Configuration["ConnectionStrings:sqlConn2"]));
        builder.Services.AddIdentity<User, IdentityRole>(_ =>
        {
            _.Password.RequiredLength = 6;
            _.Password.RequireNonAlphanumeric = true;
            _.Password.RequireLowercase = true;
            _.Password.RequireUppercase = true;
            _.Password.RequireDigit = true;

            _.User.RequireUniqueEmail = true;
            _.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

            _.SignIn.RequireConfirmedEmail = true;
        }).AddPasswordValidator<CustomIdentityValidation>()
          .AddUserValidator<CustomUserValidation>()
          .AddErrorDescriber<CustomIdentityErrorDescriber>()
          .AddEntityFrameworkStores<VoltageDbContext>()
          .AddDefaultTokenProviders();

        builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = googleAuthNSection["ClientId"]!;
            options.ClientSecret = googleAuthNSection["ClientSecret"]!;
        });


        EmailConfiguration emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        builder.Services.AddSingleton(emailConfig);
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddScoped<ISignUpService, SignUpService>();
        builder.Services.AddScoped<ILogInService, LogInService>();
        builder.Services.AddScoped<IUserModifierService, UserModifierService>();
        builder.Services.AddSignalR();
        builder.Services.AddAuthentication();
        builder.Services.AddHostedService<EmailVerifiedClearHostedService>();
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapHub<MessageHub>("/chatHub");
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "UserArea",
            pattern: "user/{controller=MainPage}/{action=Index}/{id?}",
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
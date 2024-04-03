using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Voltage.Business.Services.Concrete;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.DataBaseContext;
using Voltage.Entities.Entity;
using Voltage.Helper.Validations;
using Voltage.Services.HostedService;
using Voltage.Hubs;
using Voltage.Helper;
using Voltage.Entities.Models.HelperModels;
using Voltage.Services.Abstract;
using Voltage.Services.Concrete;

namespace Voltage;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<VoltageDbContext>(_ => _.UseSqlServer(builder.Configuration["ConnectionStrings:sqlConn2"]));
        builder.Services.Configure<DataProtectionTokenProviderOptions>(_ => _.TokenLifespan = TimeSpan.FromHours(1));
        builder.Services.ConfigureApplicationCookie(_=> _.ExpireTimeSpan = TimeSpan.FromHours(1));
        builder.Services.AddIdentity<User, IdentityRole>(_ =>
        {
            _.Password.RequiredLength = 6;
            _.Password.RequireNonAlphanumeric = true;
            _.Password.RequireLowercase = true;
            _.Password.RequireUppercase = true;
            _.Password.RequireDigit = true;

            _.Lockout.MaxFailedAccessAttempts = 3;
            _.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

            _.User.RequireUniqueEmail = true;
            _.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

            _.SignIn.RequireConfirmedEmail = true;
        }).AddPasswordValidator<CustomIdentityValidation>()
          .AddUserValidator<CustomUserValidation>()
          .AddErrorDescriber<CustomIdentityErrorDescriber>()
          .AddEntityFrameworkStores<VoltageDbContext>()
          .AddDefaultTokenProviders();

        builder.Services.AddAuthentication()
        .AddGoogle(_ =>
        {
            _.ClientId = builder.Configuration["Authentication:Google:ClientId"];
            _.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        });

        EmailConfiguration emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        builder.Services.AddSingleton(emailConfig);
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddScoped<IMessageService, MessageService>();
        builder.Services.AddScoped<ISignUpService, SignUpService>();
        builder.Services.AddScoped<ILogInService, LogInService>();
        builder.Services.AddScoped<IUserManagerService, UserManagerService>();
        builder.Services.AddScoped<ISignInManagerService, SignInManagerService>();
        builder.Services.AddScoped<IFriendListService, FriendListService>();
        builder.Services.AddAuthentication();
        builder.Services.AddHostedService<EmailVerifiedClearHostedService>();
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
        builder.Services.AddSignalR();
        builder.Services.AddSession();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        WebApplication app = builder.Build();

        if (!app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
        else
        {
            app.UseExceptionHandler("/Account/ServerError");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseStatusCodePagesWithReExecute("/Account/NotFoundView", "?statusCode={0}");
            
        app.MapHub<SignalRHub>("/signalRHub");
        app.MapHub<WebRTCHub>("/webRTCHub");
        app.UseRouting();

        app.MapControllers();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllerRoute(
            name: "UserArea",
            pattern: "user/{controller=MainPage}/{action=Index}/{id?}",
            defaults: new { area = "user" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.MapControllerRoute(
            name: "adminArea",
            pattern: "foradmin/{controller=VoltageAdmin}/{action=Index}/{id?}",
            defaults: new { area = "admin" });

        app.Run();
    }
}
using ChatGPT.Application.Portfolios;
using ChatGPTtrading.API.Definitions.Common;
using ChatGPTtrading.Domain.Config;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.API.Definitions.DbContext;

/// <summary>
/// ASP.NET Core services registration and configurations
/// </summary>
public class DbContextDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<ApplicationDbContext>(config =>
        {
            config.UseNpgsql(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)));
        });


        services.Configure<IdentityOptions>(options =>
        {
            // Default Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            // configure more options if you need
        });

        var pwdSettings = new PasswordSettings();
        builder.Configuration.GetSection(nameof(PasswordSettings)).Bind(pwdSettings);
        services.AddIdentity<ApplicationUser, AppRole>(options =>
            {
                // options.SignIn.RequireConfirmedEmail = true;
                // options.SignIn.RequireConfirmedPhoneNumber = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(pwdSettings.DefaultLockoutMinutes);
                options.Lockout.MaxFailedAccessAttempts = pwdSettings.MaxFailedAccessAttempts;
                options.Password.RequiredLength = pwdSettings.RequiredLength;
                options.Password.RequireLowercase = pwdSettings.RequireLowercase;
                options.Password.RequireUppercase = pwdSettings.RequireUppercase;
                options.Password.RequireNonAlphanumeric = pwdSettings.RequireNonAlphanumeric;
                options.Password.RequireDigit = pwdSettings.RequireDigit;
            })
            .AddSignInManager()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

    }

    public override async void ConfigureApplication(WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context!.Database.Migrate();

        var mediatr = scope.ServiceProvider.GetService<IMediator>();
        await mediatr!.Send(new SeedDataCommand());
    }
}
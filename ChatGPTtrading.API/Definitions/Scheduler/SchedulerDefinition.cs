using ChatGPTtrading.API.HostedServices;
using Hangfire;
using Hangfire.PostgreSql;

namespace ChatGPTtrading.API.Definitions.Common;

/// <summary>
/// AspNetCore common configuration
/// </summary>
public class SchedulerDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHangfire(config => {
            config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
        });
        services.AddHangfireServer();

        services.AddHostedService<ChargeProfitService>();
        services.AddHostedService<FakeStatInvestorService>();
        services.AddHostedService<FakeStatWithdrawService>();
        services.AddHostedService<FakeStatActivitiesService>();
    }

    /// <summary>
    /// Configure application for current application
    /// </summary>
    /// <param name="app"></param>
    public override void ConfigureApplication(WebApplication app)
    {
        app.UseHangfireDashboard();
    }
}
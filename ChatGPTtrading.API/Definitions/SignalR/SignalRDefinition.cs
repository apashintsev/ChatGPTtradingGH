using ChatGPT.Application.Statistics;
using ChatGPTtrading.API.SignalR;

namespace ChatGPTtrading.API.Definitions.Common;

/// <summary>
/// AspNetCore common configuration
/// </summary>
public class SignalRDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSignalR();

        services.AddScoped<IStatisticsNotificationService, StatisticsHub>();
    }

    /// <summary>
    /// Configure application for current application
    /// </summary>
    /// <param name="app"></param>
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapHub<StatisticsHub>("/stats");
    }
}
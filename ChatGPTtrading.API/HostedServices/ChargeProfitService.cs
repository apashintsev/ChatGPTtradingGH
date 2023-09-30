using Hangfire;

namespace ChatGPTtrading.API.HostedServices;
public class ChargeProfitService : BackgroundService
{
    private readonly ILogger<ChargeProfitService> _logger;

    public ChargeProfitService(ILogger<ChargeProfitService> logger)
    {
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<ChargeProfitJob>("charge-profit-job", job => job.Execute(cancellationToken), Cron.Daily(11, 00));
        _logger.LogInformation("Registered Job");
        return base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(10000); // Give Hangfire some time to complete ongoing jobs
        await base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask; // No need for this method
    }
}
using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Commands;
using MediatR;

namespace ChatGPTtrading.API.HostedServices;

public class FakeStatActivitiesService : BackgroundService
{
    private readonly ILogger<FakeStatActivitiesService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FakeStatActivitiesService(ILogger<FakeStatActivitiesService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var cmd = new AddFakeActivityCommand();
            var nextInterval = await mediator.Send(cmd);

            _logger.LogInformation($"Ждём {nextInterval}");

            await Task.Delay(nextInterval*1000, stoppingToken);
        }
    }
}

using ChatGPT.Application.Statistics.Commands;
using MediatR;

namespace ChatGPTtrading.API.HostedServices;

public class FakeStatInvestorService : BackgroundService
{
    private readonly ILogger<FakeStatInvestorService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FakeStatInvestorService(ILogger<FakeStatInvestorService> logger, IServiceScopeFactory serviceScopeFactory)
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

            var cmd = new AddFakeStatCommand(StatType.Investor);
            var fakeStats = await mediator.Send(cmd);

            var nextInterval = fakeStats.GetNextInvestorInterval();
            _logger.LogInformation($"Ждём {nextInterval}");
            if (nextInterval <= TimeSpan.Zero)
            {
                _logger.LogInformation("Нет больше инвесторов для добавления сегодня.");
            }

            await Task.Delay(nextInterval, stoppingToken);
        }
    }
}

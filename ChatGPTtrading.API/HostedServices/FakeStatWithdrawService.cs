using ChatGPT.Application.Statistics.Commands;
using MediatR;

namespace ChatGPTtrading.API.HostedServices;

public class FakeStatWithdrawService : BackgroundService
{
    private readonly ILogger<FakeStatWithdrawService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FakeStatWithdrawService(ILogger<FakeStatWithdrawService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var cmd = new AddFakeStatCommand(StatType.Withdraw);
            var fakeStats = await mediator.Send(cmd);

            var nextInterval = fakeStats.GetNextWithdrawInterval();
            _logger.LogInformation($"Ждём {nextInterval}");
            if (nextInterval <= TimeSpan.Zero)
            {
                _logger.LogInformation("Нет больше выводов для выполнения сегодня.");
            }

            await Task.Delay(nextInterval, stoppingToken);
        }
    }
}

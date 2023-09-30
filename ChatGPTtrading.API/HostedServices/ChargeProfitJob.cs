using ChatGPT.Application.Profit;
using MediatR;
public class ChargeProfitJob
{
    private readonly ILogger<ChargeProfitJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ChargeProfitJob(ILogger<ChargeProfitJob> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Execute(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Charge profit Task executed at: {time}", DateTimeOffset.UtcNow);

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var cmd = new ChargeProfitCommand();
            await mediator.Send(cmd);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Some error");
        }
    }
}
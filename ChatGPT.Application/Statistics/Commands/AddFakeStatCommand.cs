using ChatGPT.Application.Statistics.Dto;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Commands;


public enum StatType
{
    Investor,
    Withdraw
}

public record AddFakeStatCommand(StatType StatType) : IRequest<FakeStats>;

public class AddFakeStatCommandHandler : IRequestHandler<AddFakeStatCommand, FakeStats>
{
    private readonly ApplicationDbContext _context;
    private readonly IStatisticsNotificationService _statisticsNotificationService;
    private readonly ILogger<AddFakeStatCommandHandler> _logger;

    public AddFakeStatCommandHandler(ApplicationDbContext context, ILogger<AddFakeStatCommandHandler> logger, IStatisticsNotificationService statisticsNotificationService)
    {
        _context = context;
        _logger = logger;
        _statisticsNotificationService = statisticsNotificationService;
    }

    public async Task<FakeStats> Handle(AddFakeStatCommand request, CancellationToken cancellationToken)
    {
        var fakeStats = await _context.FakeStats.FirstOrDefaultAsync();

        if (fakeStats == null)
        {
            fakeStats = new FakeStats(12_000, 3500, 120_000, 20_000, 10, 100);
            _context.FakeStats.Add(fakeStats);
            await _context.SaveChangesAsync();
        }
        if (request.StatType == StatType.Investor)
        {
            fakeStats.AddInvestor();
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Время: {DateTime.UtcNow}. Добавлен 1 инвестор.");
        }
        if (request.StatType == StatType.Withdraw)
        {
            fakeStats.AddWithdraw();
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Время: {DateTime.UtcNow}. Добавлен 1 вывод.");
        }
        var stat = new StatTotalVm()
        {
            TodayInvestorsCount = fakeStats.TodayInvestorsCount,
            TotalInvestorsCount = fakeStats.TotalInvestorsCount,
            TodayWithdrawalsAmount = fakeStats.TodayWithdrawalsAmount,
            TotalWithdrawalsAmount = fakeStats.TotalWithdrawalsAmount
        };
        await _statisticsNotificationService.StatisticsUpdated(stat);
        return fakeStats;
    }
}
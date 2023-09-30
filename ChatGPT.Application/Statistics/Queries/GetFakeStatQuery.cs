using ChatGPT.Application.Statistics.Dto;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Queries;


public record GetFakeStatQuery() : IRequest<StatTotalVm>;

public class GetFakeStatQueryHandler : IRequestHandler<GetFakeStatQuery, StatTotalVm>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetFakeStatQueryHandler> _logger;

    public GetFakeStatQueryHandler(ApplicationDbContext context, ILogger<GetFakeStatQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StatTotalVm> Handle(GetFakeStatQuery request, CancellationToken cancellationToken)
    {
        var stat = await _context.FakeStats.FirstOrDefaultAsync();
        if (stat == null)
        {
            stat = new FakeStats(12_000, 3500, 120_000, 20_000, 10, 100);
        }

        return new StatTotalVm()
        {
            TodayInvestorsCount = stat.TodayInvestorsCount,
            TotalInvestorsCount = stat.TotalInvestorsCount,
            TodayWithdrawalsAmount = stat.TodayWithdrawalsAmount,
            TotalWithdrawalsAmount = stat.TotalWithdrawalsAmount
        };
    }
}

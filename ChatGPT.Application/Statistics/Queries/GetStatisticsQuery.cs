using ChatGPT.Application.Statistics.Dto;
using ChatGPTtrading.Domain.Enums;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Queries;

public record GetStatisticsQuery(bool ForLastMonth) : IRequest<StatisticsVm>;

public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsVm>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetStatisticsQueryHandler> _logger;

    public GetStatisticsQueryHandler(ApplicationDbContext context, ILogger<GetStatisticsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StatisticsVm> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        var stat = new StatisticsVm();

        stat.UsersCount = await _context.Users.Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).CountAsync();

        stat.KycApprovedUsersCount = await _context.Users.Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).Where(x => x.KycStatus).CountAsync();

        stat.Deposits = await _context.Activitys.Where(x => x.ActionType == ActionType.Deposit)
            .Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).SumAsync(x => x.Amount);

        stat.AccumulatedProfit = await _context.Activitys.Where(x => x.ActionType == ActionType.ChargeProfit)
            .Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).SumAsync(x => x.Amount);

        stat.Withdrawed = await _context.Withdrawals.Where(x => x.Status == TransactionStatus.Completed)
            .Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).SumAsync(x => x.Amount);

        stat.Reinvested = await _context.Activitys.Where(x => x.ActionType == ActionType.Reinvest)
            .Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true).SumAsync(x => x.Amount);
        stat.PayoutPercent = (await _context.PlatformSettings.FirstAsync()).ProfitRate;

        var refs = from user in _context.Users
                   from referredUser in user.Referrals
                   where !request.ForLastMonth || referredUser.ReferralUser.CreatedAt > DateTime.UtcNow.AddMonths(-1)
                   select referredUser;

        stat.ReferalsCount = await refs.CountAsync();
        var refIds = await refs.Select(x => x.ReferralUserId).ToArrayAsync();
        stat.ReferalDepos = await _context.Activitys.Where(x => x.ActionType == ActionType.Deposit)
            .Where(x => request.ForLastMonth ? x.CreatedAt >= DateTime.UtcNow.AddMonths(-1) : true)
            .Where(x => refIds.Contains(x.UserId))
            .SumAsync(x => x.Amount);
        return stat;
    }
}

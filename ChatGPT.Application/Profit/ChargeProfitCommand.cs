using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Profit;

public record ChargeProfitCommand() : IRequest;

public class ChargeProfitCommandHandler : IRequestHandler<ChargeProfitCommand>
{
    private readonly ILogger<ChargeProfitCommandHandler> _logger;
    private readonly ApplicationDbContext _context;

    public ChargeProfitCommandHandler(ILogger<ChargeProfitCommandHandler> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Handle(ChargeProfitCommand request, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Include(x => x.UserAccount)
            .Include(x => x.Activities)
            .Include(x => x.Referrals)
            .Include(x => x.ReferralProfits)
            .ToListAsync();

        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        var monthPercent = settings.ProfitRate;

        var referrals = await _context.Referrals.ToListAsync();

        foreach (var user in users)
        {
            ChargeUserProfit(user, monthPercent);

            var referral = referrals.FirstOrDefault(x => x.ReferralUserId == user.Id);
            if (referral != null)
            {
                var refUser = users.FirstOrDefault(x => x.Id == referral.UserId);
                if (refUser != null)
                {
                    ChargeReferralProfit(refUser, user, monthPercent, referral);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private void ChargeUserProfit(User user, decimal monthPercent)
    {
        var profit = (monthPercent / 30) / 100 * user.UserAccount.InvestedBalance;
        if (profit > 0)
        {
            user.ChargeProfit(profit);
        }
    }

    private void ChargeReferralProfit(User referal, User user, decimal monthPercent, Referral referralData)
    {
        var refProfit = user.UserAccount.InvestedBalance * (monthPercent / 100);
        _logger.LogInformation($"{referal.Email} получает выплату за {referralData.ReferralUser.Email} ");
        referal.AddRefPayout(referralData, refProfit, user.UserAccount.InvestedBalance);
    }

}

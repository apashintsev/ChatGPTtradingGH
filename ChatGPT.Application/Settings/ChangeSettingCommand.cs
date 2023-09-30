using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Settings;

public record ChangeSettingCommand(string Setting, string Value) : IRequest;

public class ChangeSettingCommandHandler : IRequestHandler<ChangeSettingCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ChangeSettingCommandHandler> _logger;

    public ChangeSettingCommandHandler(ApplicationDbContext context, ILogger<ChangeSettingCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ChangeSettingCommand request, CancellationToken cancellationToken)
    {
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        var fakeSettings = await _context.FakeStats.FirstOrDefaultAsync();
        if (settings == null)
        {
            throw new Exception("Settings is null. Impossible");
        }
        if (fakeSettings == null)
        {
            throw new Exception("Settings is null. Impossible");
        }

        switch (request.Setting.Replace("update-", string.Empty))
        {
            case "profit_rate":
                settings.SetProfitRate(Convert.ToDecimal(request.Value));
                break;
            case "referral_rate":
                settings.SetReferralRate(Convert.ToDecimal(request.Value));
                break;
            case "referral_threshold":
                settings.SetReferralTreshold(Convert.ToDecimal(request.Value));
                break;
            case "referral_payout":
                settings.SetRefferalPayout(Convert.ToDecimal(request.Value));
                break;
            case "hold_period_in_minutes":
                settings.SetHoldPeriodInMinutes(Convert.ToInt32(request.Value));
                break; 
            
            
            case "investors":
                fakeSettings.SetInvestorsSettings(Convert.ToInt32(request.Value));
                break;
            case "withdrawperday":
                fakeSettings.SetWithdrawalsSettings(Convert.ToDecimal(request.Value), fakeSettings.MinWithdrawalAmount, fakeSettings.MaxWithdrawalAmount);
                break;
            case "withdrawperdaymin":
                fakeSettings.SetWithdrawalsSettings(fakeSettings.NeededWithdrawalsAmount, Convert.ToDecimal(request.Value), fakeSettings.MaxWithdrawalAmount);
                break;
            case "withdrawperdaymax":
                fakeSettings.SetWithdrawalsSettings(fakeSettings.NeededWithdrawalsAmount, fakeSettings.MinWithdrawalAmount, Convert.ToDecimal(request.Value));
                break;
            case "fakeactivitysummin":
                settings.SetMinFakeActivityValue(Convert.ToDecimal(request.Value));
                break;
            case "fakeactivitysummax":
                settings.SetMaxFakeActivityValue(Convert.ToDecimal(request.Value));
                break;
            case "fadelaymin":
                settings.SetMinFakeActivityDelayInSeconds(Convert.ToInt32(request.Value));
                break;
            case "fadelaymax":
                settings.SetMaxFakeActivityDelayInSeconds(Convert.ToInt32(request.Value));
                break;


            default:
                _logger.LogWarning("Wrong setting");
                break;
        }
        await _context.SaveChangesAsync();
    }
}

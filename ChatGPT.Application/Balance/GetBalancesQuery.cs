using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Balance;


public record GetBalancesQuery(Guid UserId) : IRequest<BalanceVm>;

public class GetBalancesQueryHandler : IRequestHandler<GetBalancesQuery, BalanceVm>
{
    private readonly ILogger<GetBalancesQueryHandler> _logger;
    private readonly ApplicationDbContext _context;

    public GetBalancesQueryHandler(ILogger<GetBalancesQueryHandler> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<BalanceVm> Handle(GetBalancesQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(x => x.UserAccount).Include(x => x.Activities).FirstOrDefaultAsync(x => x.Id == request.UserId);
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();

        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }
        return new BalanceVm()
        {
            AccumulatedProfit = user.UserAccount.AccumulatedProfit,
            InvestedBalance = user.GetInvestedBalance(),
            BlockedInvestedBalance = user.GetBlockedInvestedBalance(settings.HoldPeriodInMinutes),
            InvestedBalanceCanWithdraw = user.GetInvestedBalanceThatUserCanWithdraw(settings.HoldPeriodInMinutes),
        };
    }
}

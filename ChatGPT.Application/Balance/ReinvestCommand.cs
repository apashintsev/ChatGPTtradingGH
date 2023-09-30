using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Balance
{

    public record ReinvestCommand(Guid UserId) : IRequest<BalanceVm>;

	public class ReinvestCommandHandler : IRequestHandler<ReinvestCommand, BalanceVm>
	{
		private readonly ILogger<ReinvestCommandHandler> _logger;
        private readonly ApplicationDbContext _context;

        public ReinvestCommandHandler(ILogger<ReinvestCommandHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<BalanceVm> Handle(ReinvestCommand request, CancellationToken cancellationToken)
		{
            var user = await _context.Users.Include(x=>x.UserAccount).Include(x=>x.Activities).FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }
            if (user.UserAccount.AccumulatedProfit <= 0)
            {
                throw new Exception("Нет средств для реинвестирования!");
            }

            user.Reinvest();

            await _context.SaveChangesAsync();

            var settings = await _context.PlatformSettings.FirstOrDefaultAsync();

            return new BalanceVm()
            {
                AccumulatedProfit = user.UserAccount.AccumulatedProfit,
                InvestedBalance = user.GetInvestedBalance(),
                BlockedInvestedBalance = user.GetBlockedInvestedBalance(settings.HoldPeriodInMinutes),
                InvestedBalanceCanWithdraw = user.GetInvestedBalanceThatUserCanWithdraw(settings.HoldPeriodInMinutes),
            };
        }
	}
}

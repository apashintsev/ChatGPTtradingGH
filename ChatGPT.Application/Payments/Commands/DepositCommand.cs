using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Payments.Commands
{

    public record DepositCommand(Guid UserId, decimal Amount) : IRequest<decimal>;

    public class DepositCommandHandler : IRequestHandler<DepositCommand, decimal>
    {
        private readonly ILogger<DepositCommandHandler> _logger;
        private readonly ApplicationDbContext _context;

        public DepositCommandHandler(ILogger<DepositCommandHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<decimal> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(x => x.UserAccount)
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                _logger.LogWarning("User is null");
                throw new Exception();
            }
            if (!user.KycStatus && !user.KycSended)
            {
                throw new Exception("Перед пополнением пожалуйста пройдите процедуру KYC.");
            }
            if (!user.KycStatus && user.KycSended)
            {
                throw new Exception("Пожалуйста дождитесь одобрения заявки на KYC. Среднее время ожидания 15-30 минут");
            }

            user.Deposit(request.Amount);
            var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
            //теперь надо начислить рефералу, если позволяет депозит
            if (user.UserAccount.InvestedBalance >= settings.RefferalTreshold && !user.IsReferralThresholdPayed)
            {
                //найдём того кто привёл
                var referer = await _context.Users.Include(x => x.UserAccount)
                .Include(x => x.Activities)
                .Include(x => x.Referrals)
                .FirstOrDefaultAsync(x => x.Referrals.Select(x => x.ReferralUserId).Contains(request.UserId));
                var refUser = await _context.Referrals.FirstOrDefaultAsync(x => x.ReferralUser.Id == referer.Id);
                referer.AddOneTimeRefPayout(refUser, settings.RefferalPayout, user.UserAccount.InvestedBalance);
                user.RefTresholdPayoutDone();
            }

            await _context.SaveChangesAsync();
            return user.GetInvestedBalance();
        }
    }
}

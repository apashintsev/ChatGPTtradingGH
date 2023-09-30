using ChatGPT.Application.Referals.Dtos;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Referals;


public record GetRefRewardsQuery(Guid UserId, DateTime Start, DateTime End) : IRequest<RefRewardsVm>;

public class GetRefRewardsQueryHandler : IRequestHandler<GetRefRewardsQuery, RefRewardsVm>
{
    private readonly ILogger<GetRefRewardsQueryHandler> _logger;
    private readonly ApplicationDbContext _context;

    public GetRefRewardsQueryHandler(ILogger<GetRefRewardsQueryHandler> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<RefRewardsVm> Handle(GetRefRewardsQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(x => x.ReferralProfits).FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }

        // Прямой запрос с JOIN между Profits и Referrals.
        var jointData = await (from p in _context.Profits
                               join r in _context.Referrals on p.ReferralId equals r.Id
                               where p.UserId == user.Id && p.CreatedAt.Date >= request.Start.Date && p.CreatedAt.Date <= request.End.Date
                               select new
                               {
                                   p.Profit,
                                   p.ReferalDeposit,
                                   ReferralUser = r.ReferralUserId,
                                   r.CreatedAt
                               }).OrderBy(x=>x.CreatedAt).ToListAsync();

        var refUsers = await _context.Users.Where(u => jointData.Select(jd => jd.ReferralUser).Contains(u.Id)).ToArrayAsync();

        return new RefRewardsVm()
        {
            TotalEarned = jointData.Sum(x => x.Profit),
            List = jointData.Select(x =>
            {
                var refUser = refUsers.FirstOrDefault(y => y.Id == x.ReferralUser);
                var name = $"{refUser.FirstName} {refUser.LastName}";
                return new ReferalDataVm()
                {
                    Deposit = x.ReferalDeposit,
                    Earned = x.Profit,
                    Name = string.IsNullOrWhiteSpace(name) ? refUser.Email : name,
                    Phone = refUser.Phone
                };
            }).ToList()
        };
    }
}

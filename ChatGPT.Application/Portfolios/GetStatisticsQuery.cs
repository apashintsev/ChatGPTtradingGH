using ChatGPT.Application.Portfolios.Dtos;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ChatGPT.Application.Portfolios
{

    public record GetStatisticsQuery(Guid UserId, string DateFilter) : IRequest<StatisticsVm>;

    public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsVm>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetStatisticsQueryHandler> _logger;

        public GetStatisticsQueryHandler(ILogger<GetStatisticsQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<StatisticsVm> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(x => x.Activities).Include(x => x.UserAccount).FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }
            var earnings = user.Activities.Where(x => x.ActionType == ChatGPTtrading.Domain.Enums.ActionType.ChargeProfit);
            var ret = new StatisticsVm()
            {
                InvestmentBalance = user.UserAccount.InvestedBalance,
                TotalEarnings = earnings.Sum(x => x.Amount),
                EarningsLast30Days = earnings.Where(x => x.CreatedAt > DateTime.UtcNow.AddDays(-30)).Sum(x => x.Amount),
                Dates = earnings.Select(x => $"01.{x.CreatedAt.ToString("MM.yyyy")}-01.{x.CreatedAt.AddMonths(1).ToString("MM.yyyy")}").Distinct().ToList(),
                //Data = earnings.Select(new )
            };

            if (!string.IsNullOrWhiteSpace(request.DateFilter))
            {
                var startAt = request.DateFilter.Split('-')[0];
                var endAt = request.DateFilter.Split('-')[1];
                earnings = earnings.Where(x => x.CreatedAt >= DateTime.Parse(startAt) && x.CreatedAt <= DateTime.Parse(endAt));
            }
            else
            {
                earnings = earnings.Where(x => x.CreatedAt > DateTime.UtcNow.AddMonths(-1));
            }

            ret.Data = earnings.GroupBy(x => x.CreatedAt.ToString("MMM", CultureInfo.CreateSpecificCulture("ru")))
            .Select(x => new DiagramDataVm() { Name = x.Key, Value = x.Sum(y => y.Amount) }).ToList();

            return ret;
        }
    }
}

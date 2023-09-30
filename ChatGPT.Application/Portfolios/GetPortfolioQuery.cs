using ChatGPT.Application.Portfolios.Dtos;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Portfolios
{

    public record GetPortfolioQuery() : IRequest<IEnumerable<PortfolioVm>>;

	public class GetPortfolioQueryHandler : IRequestHandler<GetPortfolioQuery, IEnumerable<PortfolioVm>>
	{
		private readonly ILogger<GetPortfolioQueryHandler> _logger;

		private readonly ApplicationDbContext _context;

        public GetPortfolioQueryHandler(ILogger<GetPortfolioQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<PortfolioVm>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
		{
			return await _context.Portfolios.OrderByDescending(x=>x.Percentage).Select(x=> new PortfolioVm()
			{
				Icon= x.Icon,
				Currency = x.Currency,
				Percentage= x.Percentage
			}).ToArrayAsync();
		}
	}
}

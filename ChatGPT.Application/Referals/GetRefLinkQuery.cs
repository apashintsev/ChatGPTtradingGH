using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Referals
{

    public record GetRefLinkQuery(Guid UserId) : IRequest<string>;

	public class GetRefLinkQueryHandler : IRequestHandler<GetRefLinkQuery, string>
	{
		private readonly ILogger<GetRefLinkQueryHandler> _logger;
        private readonly ApplicationDbContext _context;

        public GetRefLinkQueryHandler(ILogger<GetRefLinkQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<string> Handle(GetRefLinkQuery request, CancellationToken cancellationToken)
		{
            var user = await _context.Users.Include(x => x.UserAccount).FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }
            return user.ReferralLink;
        }
	}
}

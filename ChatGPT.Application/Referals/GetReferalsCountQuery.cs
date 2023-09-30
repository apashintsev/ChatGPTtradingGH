using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Referals
{

    public record GetReferalsCountQuery(Guid UserId) : IRequest<int>;

	public class GetReferalsCountQueryHandler : IRequestHandler<GetReferalsCountQuery, int>
	{
		private readonly ILogger<GetReferalsCountQueryHandler> _logger;
        private readonly ApplicationDbContext _context;

        public GetReferalsCountQueryHandler(ILogger<GetReferalsCountQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<int> Handle(GetReferalsCountQuery request, CancellationToken cancellationToken)
		{
            var user = await _context.Users.Include(x => x.Referrals).FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }

            return user.Referrals.Count;
        }
	}
}

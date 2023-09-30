using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Kyc
{

    public record GetIsNeedSendKycQuery(Guid UserId) : IRequest<bool>;

	public class GetIsNeedSendKycQueryHandler : IRequestHandler<GetIsNeedSendKycQuery, bool>
	{
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetIsNeedSendKycQueryHandler> _logger;

        public GetIsNeedSendKycQueryHandler(ILogger<GetIsNeedSendKycQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> Handle(GetIsNeedSendKycQuery request, CancellationToken cancellationToken)
		{
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }
            return !user.KycSended;
        }
	}
}

using ChatGPT.Application.Referals.Dtos;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Referals
{

    public record GetRefSettingsQuery() : IRequest<RefSettingsVm>;

	public class GetRefSettingsQueryHandler : IRequestHandler<GetRefSettingsQuery, RefSettingsVm>
	{
		private readonly ILogger<GetRefSettingsQueryHandler> _logger;
        private readonly ApplicationDbContext _context;

        public GetRefSettingsQueryHandler(ILogger<GetRefSettingsQueryHandler> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<RefSettingsVm> Handle(GetRefSettingsQuery request, CancellationToken cancellationToken)
		{
            var settings = await _context.PlatformSettings.FirstOrDefaultAsync();

            return new RefSettingsVm()
            {
                Percent = settings.ReferralRate,
                Reward = settings.RefferalPayout,
                Treshold = settings.RefferalTreshold,
            };
        }
	}
}

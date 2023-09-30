using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Settings;


public record GetPlatformSettingsQuery() : IRequest<PlatformSettings>;

public class GetPlatformSettingsQueryHandler : IRequestHandler<GetPlatformSettingsQuery, PlatformSettings>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetPlatformSettingsQueryHandler> _logger;

    public GetPlatformSettingsQueryHandler(ApplicationDbContext context, ILogger<GetPlatformSettingsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PlatformSettings> Handle(GetPlatformSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            throw new Exception("Settings is null. Impossible");
        }
        return settings;
    }
}

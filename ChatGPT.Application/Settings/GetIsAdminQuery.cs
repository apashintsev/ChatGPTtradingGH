using ChatGPTtrading.Domain.Config;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatGPT.Application.Settings;


public record GetIsAdminQuery(long ChatId) : IRequest<bool>;

public class GetIsAdminQueryHandler : IRequestHandler<GetIsAdminQuery, bool>
{
    private readonly TelegramSettings _settings;
    private readonly ILogger<GetIsAdminQueryHandler> _logger;

    public GetIsAdminQueryHandler(IOptions<TelegramSettings> settings, ILogger<GetIsAdminQueryHandler> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> Handle(GetIsAdminQuery request, CancellationToken cancellationToken)
    {
        return _settings.Admins.Contains(request.ChatId);
    }
}

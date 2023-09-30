using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ChatGPT.Application.Kyc;


public record ApproveKycCommand(Guid UserId, bool IsApproved, long ChatId, int MessageId) : IRequest;

public class ApproveKycCommandHandler : IRequestHandler<ApproveKycCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ApproveKycCommandHandler> _logger;

    public ApproveKycCommandHandler(ApplicationDbContext context, ILogger<ApproveKycCommandHandler> logger, ITelegramBotClient botClient)
    {
        _context = context;
        _logger = logger;
        _botClient = botClient;
    }

    public async Task Handle(ApproveKycCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }
        try
        {
            var editedMessage = await _botClient.EditMessageReplyMarkupAsync(
                chatId: request.ChatId,
                messageId: request.MessageId,
                replyMarkup: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Cant edit message;", ex);
        }

        if (request.IsApproved)
        {
            user.AcceptKyc();
        }
        else
        {
            user.RejectKyc();
        }

        await _context.SaveChangesAsync();
    }
}

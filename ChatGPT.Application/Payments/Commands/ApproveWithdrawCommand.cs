using ChatGPT.Application.Payments.Events;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ChatGPT.Application.Payments.Commands;


public record ApproveWithdrawCommand(Guid TransactionId, bool IsApproved, long ChatId, int MessageId) : IRequest;

public class ApproveWithdrawCommandHandler : IRequestHandler<ApproveWithdrawCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ApproveWithdrawCommandHandler> _logger;

    public ApproveWithdrawCommandHandler(ApplicationDbContext context, ILogger<ApproveWithdrawCommandHandler> logger, IMediator mediator, ITelegramBotClient botClient)
    {
        _context = context;
        _logger = logger;
        _mediator = mediator;
        _botClient = botClient;
    }

    public async Task Handle(ApproveWithdrawCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Withdrawals.FirstOrDefaultAsync(x => x.Id == request.TransactionId);
        if (transaction == null)
        {
            //TODO real exception
            throw new Exception("Transaction not found");
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
            transaction.Confirm();
        }
        else
        {
            transaction.Reject();
        }
        await _context.SaveChangesAsync();

        await _mediator.Publish(new WithdrawApprovedEvent(transaction.Id));
    }
}

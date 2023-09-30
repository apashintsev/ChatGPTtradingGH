using ChatGPTtrading.Domain.Config;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPT.Application.Payments.Events;

public record WithdrawRequestedEvent(
    Guid UserId,
    Guid TransactionId,
    string Email,
    string Phone,
    string FirstName,
    string LastName,
    decimal Amount,
    string Currency,
    DateTime CreatedAt) : INotification;

public class WithdrawRequestedEventHandler : INotificationHandler<WithdrawRequestedEvent>
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramSettings _settings;
    private readonly ILogger<WithdrawRequestedEventHandler> _logger;

    public WithdrawRequestedEventHandler(ITelegramBotClient botClient, IOptions<TelegramSettings> settings, ILogger<WithdrawRequestedEventHandler> logger)
    {
        _botClient = botClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Handle(WithdrawRequestedEvent notification, CancellationToken cancellationToken)
    {
        var msg = $"""
                <b>Поступила заявка на вывод.</b>
                <b>Почта:</b> {notification.Email};
                <b>Телефон:</b> {notification.Phone};
                <b>Имя:</b> {notification.FirstName};
                <b>Фамилия:</b> {notification.LastName};

                <b>Ид транзакции:</b> {notification.TransactionId};
                <b>Сумма:</b> {notification.Amount};
                <b>Валюта:</b> {notification.Currency};
                <b>Дата создания:</b> {notification.CreatedAt:yyyy-MM-dd HH:mm:ss};
                """;

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Approve", $"approvewithdraw:{notification.TransactionId}"),
                InlineKeyboardButton.WithCallbackData("Reject", $"rejectwithdraw:{notification.TransactionId}")
            }
        });
        try
        {
            await _botClient.SendTextMessageAsync(_settings.WithdrawChat, msg, replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with send message to {_settings.WithdrawChat}", ex);
        }
    }
}

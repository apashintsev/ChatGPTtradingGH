using ChatGPTtrading.Domain.Config;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPT.Application.Kyc;

public record KycDataSendedEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Residency,
    string Citizenship,
    string Occupation,
    string TemporaryResidence,
    string AvatarId,
    string PassportId,
    string PhotoWithPassportId
) : INotification;

public class KycDataSendedEventHandler : INotificationHandler<KycDataSendedEvent>
{
    private readonly ITelegramBotClient _botClient;
    private readonly TelegramSettings _settings;
    private readonly ILogger<KycDataSendedEventHandler> _logger;


    public KycDataSendedEventHandler(ITelegramBotClient botClient, IOptions<TelegramSettings> settings, ILogger<KycDataSendedEventHandler> logger)
    {
        _botClient = botClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Handle(KycDataSendedEvent notification, CancellationToken cancellationToken)
    {
        var storageUrl = "https://storage.yandexcloud.net/cdn.chatgpt/";
        var msg = $"""
                <b>Поступили данные KYC</b>.
                <b>Имя:</b> {notification.FirstName};
                <b>Фамилия:</b> {notification.LastName};
                <b>Почта:</b> {notification.Email};
                <b>Телефон:</b> {notification.Phone};
                <b>Место жительства:</b> {notification.Residency};
                <b>Гражданство:</b> {notification.Citizenship};
                <b>Вид деятельности:</b> {notification.Occupation};
                <b>ВНЖ:</b> {notification.TemporaryResidence};
                <a href="{storageUrl + notification.PassportId}"><b>Паспорт</b></a>;
                <a href="{storageUrl + notification.PhotoWithPassportId}"><b>C паспортом</b></a>,
                """;

        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Approve", $"approvekyc:{notification.UserId}"),
                InlineKeyboardButton.WithCallbackData("Reject", $"rejectkyc:{notification.UserId}")
            }
        });
        try
        {
            await _botClient.SendTextMessageAsync(_settings.KycChat, msg, replyMarkup: keyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with send message to {_settings.KycChat}", ex);
        }
    }
}

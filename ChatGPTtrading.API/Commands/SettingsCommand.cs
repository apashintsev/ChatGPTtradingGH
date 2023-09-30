using ChatGPT.Application.Settings;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPTtrading.API.Commands;

public class SettingsCommand : TelegramCommand
{
    public override string Name { get; } = "Настройки";

    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var isAdmin = await mediator.Send(new GetIsAdminQuery(chatId), cancellationToken);
        var settingsData = await mediator.Send(new GetPlatformSettingsQuery());

        var reMessage = $"""
            profit_rate: {settingsData.ProfitRate}
            referral_rate: {settingsData.ReferralRate}
            referral_threshold: {settingsData.RefferalTreshold}
            referral_payout: {settingsData.RefferalPayout}
            hold_period_in_minutes: {settingsData.HoldPeriodInMinutes}
            """;

        IReplyMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(
        new[]
                {
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить profit_rate","update-profit_rate"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить referral_rate","update-referral_rate"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить referral_threshold","update-referral_threshold"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить referral_payout","update-referral_payout"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить hold_period_in_minutes","update-hold_period_in_minutes"),
                  },
                }
        );
        await client.SendTextMessageAsync(chatId, isAdmin ? reMessage: "Только для админов",
            parseMode: ParseMode.Html, replyMarkup: isAdmin ? inlineKeyboardMarkup:null, disableWebPagePreview: true);
    }

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}
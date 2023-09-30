using ChatGPT.Application.Settings;
using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPTtrading.API.Commands;

public class AddBalanceCommand : TelegramCommand
{
    public override string Name { get; } = "Добавить баланс";

    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var isAdmin = await mediator.Send(new GetIsAdminQuery(chatId), cancellationToken);;

        var reMessage = $"""
            Укажите Email пользователя, которому нужно изменить баланс и через | сумму.
            Пример: test@email.com|500
            (Будет начислено 500 USD юзеру с почтой test@email.com)
            """;

        IReplyMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(
        new[]
                {
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Ввести данные","addbalance"),
                  },
                }
        );
        await client.SendTextMessageAsync(chatId, isAdmin ? reMessage : "Только для админов",
            parseMode: ParseMode.Html, replyMarkup: isAdmin ? inlineKeyboardMarkup : null, disableWebPagePreview: true);
    }

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}
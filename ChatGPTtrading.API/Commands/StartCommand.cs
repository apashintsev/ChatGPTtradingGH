using ChatGPT.Application.Settings;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatGPTtrading.API.Commands;

public class StartCommand : TelegramCommand
{
    public override string Name => @"/start";

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }

    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var isAdmin = await mediator.Send(new GetIsAdminQuery(chatId), cancellationToken);
        await client.SendTextMessageAsync(
            chatId: chatId,
            text: isAdmin ? "Добро пожаловать!" : "Только для админов",
            parseMode: ParseMode.Html,
            disableNotification: false,
            replyToMessageId: 0,
            replyMarkup: isAdmin ? ReplyKeyboardMarkup : null,
            cancellationToken: cancellationToken);
    }
}
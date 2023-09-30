using ChatGPT.Application.Profit;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatGPTtrading.API.Commands;

public class ChargeProfitBotCommand : TelegramCommand
{
    public override string Name => @"Начислить";

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }

    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

         await mediator.Send(new ChargeProfitCommand(), cancellationToken);

    }
}
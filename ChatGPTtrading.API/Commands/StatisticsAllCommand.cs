using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatGPTtrading.API.Commands;

public class StatisticsAllCommand : TelegramCommand
{
    public override string Name { get; } = "Статистика за всё время";
    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var data = await mediator.Send(new GetStatisticsQuery(false));

        var reMessage = $"""
            Кол-во новых пользователей:{data.UsersCount}
            Кол-во новых пользователей успешно прошедших KYC: {data.KycApprovedUsersCount}
            Сумма депозитов: {(int)data.Deposits}
            Сумма накопленных прибылей:{(int)data.AccumulatedProfit}
            Сумма выплаченных средств:{(int)data.Withdrawed}
            Реинвестировано: {(int)data.Reinvested}
            Текущий % прибыли на вклады пользователей: {data.PayoutPercent}
            Количество приглашенных новых пользователей через реферальные ссылки:{data.ReferalsCount}
            Oбщее кол-во депозитов приглашенных пол-лей: {data.ReferalDepos}
            """;

        await client.SendTextMessageAsync(chatId, reMessage,
            parseMode: ParseMode.Html, replyMarkup: base.ReplyKeyboardMarkup, disableWebPagePreview: true);
    }

    public override bool Contains(Message message)
    {
        if (message.Type != MessageType.Text)
            return false;

        return message.Text.Contains(Name);
    }
}
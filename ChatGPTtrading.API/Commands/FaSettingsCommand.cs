using ChatGPT.Application.Settings;
using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPTtrading.API.Commands;

public class FaSettingsCommand : TelegramCommand
{
    public override string Name { get; } = "Управление фейковой активностью";

    public override async Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var isAdmin = await mediator.Send(new GetIsAdminQuery(chatId), cancellationToken);
        var settingsData = await mediator.Send(new GetPlatformSettingsQuery());
        var fakeStat = await mediator.Send(new GetFakeStatFullQuery());

        var reMessage = $"""
            Кол-во инвесторов в день: {fakeStat.NeededInvestorsCount} (сегодня добавлено {fakeStat.TotalInvestorsCount})
            Суммарно выплат в день на:{fakeStat.NeededWithdrawalsAmount} (сегодня уже на {fakeStat.TotalWithdrawalsAmount})
            Диапазон сумм выплат от {fakeStat.MinWithdrawalAmount} до {fakeStat.MaxWithdrawalAmount}
            Мин  сумма в фейковой активности: {settingsData.MinFakeActivityValue}
            Макс сумма в фейковой активности: {settingsData.MaxFakeActivityValue}
            Мин  задержка перед доб. новой фейковой активности (сек): {settingsData.MinFakeActivityDelayInSeconds}
            Макс задержка перед доб. новой фейковой активности (сек): {settingsData.MaxFakeActivityDelayInSeconds}
            """;

        IReplyMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(
        new[]
                {
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Кол-во инвесторов в день","update-investors"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Суммарно выплат в день","update-withdrawperday"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Диапазон сумм выплат от","update-withdrawperdaymin"),
                  },
                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Диапазон сумм выплат до","update-withdrawperdaymax"),
                  },

                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Мин  сумма в фейковой активности","update-fakeactivitysummin"),
                  },
                                    new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Макс  сумма в фейковой активности","update-fakeactivitysummax"),
                  },


                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Мин  задержка перед доб. новой ФА","update-fadelaymin"),
                  },

                  new[]
                  {
                    InlineKeyboardButton.WithCallbackData("Изменить Мax  задержка перед доб. новой ФА","update-fadelaymax"),
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
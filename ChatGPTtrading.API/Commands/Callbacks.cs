using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using ChatGPT.Application.Kyc;
using ChatGPT.Application.Payments;
using ChatGPT.Application.Payments.Commands;

namespace ChatGPTtrading.API.Commands;

public class Callbacks
{
    private readonly IMediator _mediator;

    public static Dictionary<long, string> UserStates = new Dictionary<long, string>();

    public Callbacks(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProcessAsync(CallbackQuery callbackQuery, ITelegramBotClient client)
    {
        if (callbackQuery.Data.Contains("update"))
        {
            var callbackData = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            // Устанавливаем состояние пользователя в выбранное значение
            UserStates[chatId] = callbackData;

            //await client.EditMessageTextAsync(chatId, messageId, $"Вы выбрали: {callbackData}");
            await client.SendTextMessageAsync(chatId, "Введите новое значение:");
        }
        if (callbackQuery.Data.Contains("addbalance"))
        {
            var callbackData = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;
            var messageId = callbackQuery.Message.MessageId;

            // Устанавливаем состояние пользователя в выбранное значение
            UserStates[chatId] = callbackData;

            //await client.EditMessageTextAsync(chatId, messageId, $"Вы выбрали: {callbackData}");
            await client.SendTextMessageAsync(chatId, "Введите новое значение в формате email|сумма:");
        }
        if (callbackQuery.Data.Contains("approvekyc:") || callbackQuery.Data.Contains("rejectkyc:"))
        {
            await _mediator.Send(new ApproveKycCommand(Guid.Parse(callbackQuery.Data.Split(":")[1]), callbackQuery.Data.Contains("approvekyc:"),
                 callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId));
        }
        if (callbackQuery.Data.Contains("approvewithdraw:") || callbackQuery.Data.Contains("rejectwithdraw:"))
        {
            await _mediator.Send(new ApproveWithdrawCommand(Guid.Parse(callbackQuery.Data.Split(":")[1]), callbackQuery.Data.Contains("approvewithdraw:"),
                callbackQuery.Message!.Chat.Id, callbackQuery.Message!.MessageId));
        }

        //if (_answers.TryGetValue(callbackQuery.Data, out string answer))
        //{
        //    if (answer.Contains("Command"))
        //    {
        //        TelegramCommand cmd = null;
        //        switch (answer)
        //        {
        //            case "PayCommand":
        //                cmd = new PayCommand();
        //                break;
        //                // Add additional cases for other command options if needed
        //        }

        //        if (cmd is not null)
        //        {
        //            CancellationToken cancellationToken = new();
        //            var msgOverride = callbackQuery.Message;
        //            msgOverride.From = callbackQuery.From; //чтобы выполнять команды не от имени бота, а от имени юзера
        //            await cmd.Execute(msgOverride, client, _mediator, cancellationToken);
        //        }
        //    }
        //    else if (answer.Contains("requestpayout"))
        //    {
        //        await _mediator.Send(new RequestPayoutCommand(callbackQuery.From.Id));
        //    }

        //    else if (answer.Contains("sometext"))
        //    {
        //        await client.AnswerCallbackQueryAsync(
        //            callbackQueryId: callbackQuery.Id,
        //            text: $"You clicked on the button! Data: {callbackQuery.Data}",
        //            showAlert: true // this will show a popup message
        //        );
        //    }


        //    else
        //    {
        //        await client.SendTextMessageAsync(callbackQuery.Message.Chat.Id, answer);
        //    }
        //}
    }
}

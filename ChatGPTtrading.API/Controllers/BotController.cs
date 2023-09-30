using ChatGPT.Application.Settings;
using ChatGPTtrading.API.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
namespace ChatGPTtrading.API.Controllers;

[ApiController]
[Route("api/message/update")]
public class BotController : Controller
{
    private IMediator _mediator;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ICommandService _commandService;

    public BotController(ICommandService commandService, ITelegramBotClient telegramBotClient, IMediator mediator)
    {
        _commandService = commandService;
        _telegramBotClient = telegramBotClient;
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Started");
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update == null) return Ok();

        var message = update.Message;
        var chatId = update.Message?.Chat.Id;
        switch (update.Type)
        {
            case Telegram.Bot.Types.Enums.UpdateType.Unknown:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.Message:

                var cancelationToken = new CancellationToken();
                foreach (var command in _commandService.GetCommands())
                {
                    if (command.Contains(message))
                    {
                        await command.Execute(message, _telegramBotClient, _mediator, cancelationToken);
                        break;
                    }
                }
                if (chatId.HasValue && Callbacks.UserStates.ContainsKey(chatId.Value))
                {
                    if (Callbacks.UserStates[chatId.Value].Contains("addbalance"))
                    {
                        var elements = message.Text.Split("|");
                        if (elements.Length == 2)
                        {
                            await _mediator.Send(new ChatGPT.Application.Balance.AddBalanceCommand(
                                message.Text.Split("|")[0], 
                                Convert.ToDecimal(message.Text.Split("|")[1])));
                            await _telegramBotClient.SendTextMessageAsync(chatId, $"Баланс пользователя увеличен.");
                        }
                        else
                        {
                            await _telegramBotClient.SendTextMessageAsync(chatId, $"Некорректный формат.");
                        }
                    }
                    else
                    {
                        await _mediator.Send(new ChangeSettingCommand(Callbacks.UserStates[chatId.Value], message.Text));
                        await _telegramBotClient.SendTextMessageAsync(chatId, $"Значение для {Callbacks.UserStates[chatId.Value]} успешно обновлено на {message.Text}.");
                    }

                    Callbacks.UserStates.Remove(chatId.Value);
                }

                break;
            case Telegram.Bot.Types.Enums.UpdateType.InlineQuery:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                var clbk = new Callbacks(_mediator);
                await clbk.ProcessAsync(update.CallbackQuery, _telegramBotClient);
                break;
            case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.ChannelPost:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.ShippingQuery:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.PreCheckoutQuery:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.Poll:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.PollAnswer:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.MyChatMember:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.ChatMember:
                break;
            case Telegram.Bot.Types.Enums.UpdateType.ChatJoinRequest:
                break;
            default:
                break;
        }



        return Ok();
    }
}
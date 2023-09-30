using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChatGPTtrading.API.Commands;

public abstract class TelegramCommand
{
    public abstract string Name { get; }

    public abstract Task Execute(Message message, ITelegramBotClient client, IMediator mediator, CancellationToken cancellationToken);

    public abstract bool Contains(Message message);

    public virtual ReplyKeyboardMarkup ReplyKeyboardMarkup { get; private set; }

    public virtual string Message { get; set; }

    public TelegramCommand()
    {
        ReplyKeyboardMarkup = new ReplyKeyboardMarkup
            (new[]
                {
                    new[]
                    {
                     new KeyboardButton("Настройки"),
                    },
                    new[]
                    {
                     new KeyboardButton("Управление фейковой активностью"),
                    },
                    new[]
                    {
                     new KeyboardButton("Статистика за месяц"),
                    },
                    new[]
                    {
                     new KeyboardButton("Статистика за всё время"),
                    },
                    new[]
                    {
                     new KeyboardButton("Начислить"),
                    },
                    new[]
                    {
                     new KeyboardButton("Добавить баланс"),
                    },
                }
            )
        {
            ResizeKeyboard = true
        };
        Message = "Ответ на команду: " + Name;
    }
}
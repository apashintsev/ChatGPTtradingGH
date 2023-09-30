using ChatGPTtrading.Domain.Config;
using Microsoft.Extensions.Options;

namespace ChatGPTtrading.API.Commands;

public interface ICommandService
{
    List<TelegramCommand> GetCommands();
}
public class CommandService : ICommandService
{
    private readonly List<TelegramCommand> _commands;

    public CommandService()
    {
        _commands = new List<TelegramCommand>
        {
            new SettingsCommand(),
            new StartCommand(),
            new StatisticsOneMonthCommand(),
            new StatisticsAllCommand(),
            new ChargeProfitBotCommand(),
            new FaSettingsCommand(),
            new AddBalanceCommand()
        };
    }

    public List<TelegramCommand> GetCommands() => _commands;
}
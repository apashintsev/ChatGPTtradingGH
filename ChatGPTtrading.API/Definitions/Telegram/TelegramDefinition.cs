using ChatGPTtrading.API.Commands;
using ChatGPTtrading.Domain.Config;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace ChatGPTtrading.API.Definitions.Common;

/// <summary>
/// AspNetCore common configuration
/// </summary>
public class TelegramDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        var telegramSettings = builder.Configuration.GetSection("TelegramSettings").Get<TelegramSettings>();
        builder.Services.AddScoped<ICommandService, CommandService>();
        var client = new TelegramBotClient(telegramSettings.BotToken);
        var webHook = $"{telegramSettings.Url}/api/message/update";
        client.SetWebhookAsync(webHook).GetAwaiter().GetResult();
        services.AddTransient<ITelegramBotClient>(x => client);
    }
}

//ngrok http https://localhost:7172
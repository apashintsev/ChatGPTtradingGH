using ChatGPTtrading.API.Definitions.Common;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using TelegramSink;

namespace ChatGPTtrading.API.Definitions.Serilog;

public class SerilogDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.TeleSink(
                          telegramApiKey: "2052504528:AAFYCA0u3cihBljQwP-lMyg6qIf5to7LQbw",
                          telegramChatId: "326065877")
                .WriteTo.File("/log/logs.log",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 10485760);
        });
    }
}

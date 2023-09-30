using ChatGPTtrading.Domain.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGPTtrading.Infrastructure;

public static class ConfigMappingExtension
{
    public static void AddConfigMapping(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<PasswordSettings>(configuration.GetSection(nameof(PasswordSettings)));
        services.Configure<TelegramSettings>(configuration.GetSection(nameof(TelegramSettings)));
        services.Configure<PaymentServiceConfig>(configuration.GetSection(nameof(PaymentServiceConfig)));
    }
}

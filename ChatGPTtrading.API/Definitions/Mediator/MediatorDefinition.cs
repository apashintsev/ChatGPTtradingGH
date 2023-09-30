using ChatGPT.Application.Auth.Commands;
using ChatGPT.Application.Behaviors;
using ChatGPT.Application.Behaviors.Transaction;
using ChatGPTtrading.API.Definitions.Common;
using MediatR;

namespace ChatGPTtrading.API.Definitions.Mediator;

/// <summary>
/// Register Mediator as application definition
/// </summary>
public class MediatorDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        services.AddTransactionsBehavior();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<LoginCommand>());
    }
}
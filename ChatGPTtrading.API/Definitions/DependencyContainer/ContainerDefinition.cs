using ChatGPTtrading.API.Definitions.Common;
using ChatGPTtrading.Infrastructure.Services;
using ChatGPTtrading.Infrastructure.Services.Interfaces;

namespace ChatGPTtrading.API.Definitions.DependencyContainer;

/// <summary>
/// Dependency container definition
/// </summary>
public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileUploadService, FileUploadService>();

        //services.AddTransient<
        //    IPipelineBehavior<CategoryUpdateRequest, OperationResult<CategoryEditViewModel>>,
        //    CategoryUpdateRequestTransactionBehavior>();
    }
}
namespace ChatGPTtrading.API.Definitions.Common
{
    internal interface IAppDefinition
    {
        int OrderIndex { get; }

        bool Enabled { get; }

        void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder);

        void ConfigureApplication(WebApplication app);
    }
}

namespace ChatGPTtrading.API.Definitions.Common
{
    public abstract class AppDefinition: IAppDefinition
    {
        public virtual int OrderIndex => 0;

        public virtual bool Enabled { get; protected set; } = true;
        /// <summary>
        /// Этот метод предназначен для настройки сервисов приложения.
        /// </summary>
        /// <param name="services">Коллекция сервисов для настройки.</param>
        /// <param name="builder">Строитель веб-приложения, который может быть использован для дополнительной настройки.</param>
        public virtual void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder) { }

        /// <summary>
        /// Этот метод предназначен для конфигурации конвейера обработки запросов приложения.
        /// </summary>
        /// <param name="app">Построитель приложения, который используется для настройки конвейера обработки запросов.</param>
        public virtual void ConfigureApplication(WebApplication app) { }
    }
}

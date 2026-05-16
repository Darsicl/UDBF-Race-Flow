using Serilog;

namespace UDBFRaceFlow.WebApi.Extensions
{
    public static class ConfigureHostBuilderExtensions
    {
        public static void ConfigureSerilog(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, services, loggerCofiguration) =>
            {
                loggerCofiguration.ReadFrom.Configuration(builder.Configuration);
            });
        }
    }
}

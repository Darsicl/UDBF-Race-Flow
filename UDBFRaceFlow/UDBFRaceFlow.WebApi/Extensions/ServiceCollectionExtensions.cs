using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Infrastructure.Persistence;

namespace UDBFRaceFlow.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<RaceDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("RaceDbConnection"));
            });
        }
    }
}

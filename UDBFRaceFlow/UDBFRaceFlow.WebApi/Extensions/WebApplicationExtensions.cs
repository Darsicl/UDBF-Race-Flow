using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Infrastructure.Persistence;

namespace UDBFRaceFlow.WebApi.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                RaceDbContext context = services.GetRequiredService<RaceDbContext>();
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during startup migration");
            }


        }
    }
}

using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UDBFRaceFlow.Application.Mapping;
using UDBFRaceFlow.Application.Services.Race.Create.RaceSystems.SystemA;
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

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddValidatorsFromAssemblyContaining<SystemADtoValidator>();

            TypeAdapterConfig.GlobalSettings.Scan(typeof(CreateRaceMappingConfig).Assembly);
        }
    }
}

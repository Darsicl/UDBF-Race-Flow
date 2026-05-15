using Microsoft.EntityFrameworkCore;
using UDBFRaceFlow.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<RaceDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("RaceDbConnection"));
});

WebApplication app = builder.Build();

app.Run();

using UDBFRaceFlow.WebApi.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.ConfigureSerilog(builder);

WebApplication app = builder.Build();

await app.ApplyMigrations();

app.Run();

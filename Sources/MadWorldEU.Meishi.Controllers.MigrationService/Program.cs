using MadWorldEU.Meishi.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMigrationService(builder.Configuration);

var host = builder.Build();
await host.RunAsync();

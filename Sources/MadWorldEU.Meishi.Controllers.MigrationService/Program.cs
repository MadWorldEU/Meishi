using MadWorldEU.Meishi;
using MadWorldEU.Meishi.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPostgresql(builder.Configuration);
builder.Services.AddHostedService<DatabaseMigrator>();

var host = builder.Build();
await host.RunAsync();

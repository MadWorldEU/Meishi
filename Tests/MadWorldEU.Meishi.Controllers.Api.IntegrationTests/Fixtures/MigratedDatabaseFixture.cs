extern alias MigrationService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using MigrationService::MadWorldEU.Meishi.Extensions;

namespace MadWorldEU.Meishi.Fixtures;

/// <summary>
/// Starts a PostgreSQL database in a container and runs the migration service against it,
/// so the database is migrated and seeded with the default curriculum vitae before the tests run.
/// </summary>
public sealed class MigratedDatabaseFixture : IAsyncLifetime
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await RunMigrationServiceAsync();
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    private async Task RunMigrationServiceAsync()
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:meishi-db"] = ConnectionString
        });
        builder.Services.AddMigrationService(builder.Configuration);

        using var host = builder.Build();
        using var timeout = new CancellationTokenSource(Timeout);

        // The migrator reports failures through the process exit code, so reset it before and after the run.
        Environment.ExitCode = 0;
        await host.RunAsync(timeout.Token);
        var exitCode = Environment.ExitCode;
        Environment.ExitCode = 0;

        if (timeout.IsCancellationRequested)
        {
            throw new TimeoutException("The migration service did not stop on its own.");
        }

        if (exitCode != 0)
        {
            throw new InvalidOperationException($"The migration service failed with exit code {exitCode}.");
        }
    }
}

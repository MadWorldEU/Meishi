using MadWorldEU.Meishi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace MadWorldEU.Meishi;

/// <summary>
/// Runs the migration service against a real PostgreSQL database in a container.
/// xUnit creates a new instance per test, so every test starts with an empty database.
/// </summary>
public sealed class DatabaseMigratorTests : IAsyncLifetime
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task Run_WhenDatabaseIsEmpty_AppliesAllMigrations()
    {
        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await AssertAllMigrationsAppliedAsync();
    }

    [Fact]
    public async Task Run_WhenMigrationsAreAlreadyApplied_Succeeds()
    {
        // Arrange
        await RunMigrationServiceAsync();

        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await AssertAllMigrationsAppliedAsync();
    }

    [Fact]
    public async Task Run_WhenDatabaseIsOffline_ExitsWithErrorCode()
    {
        // Arrange
        // Read the connection string first, a stopped container no longer has a mapped port
        var connectionString = _postgres.GetConnectionString();
        await _postgres.StopAsync();

        // Act
        var exitCode = await RunMigrationServiceAsync(connectionString);

        // Assert
        Assert.Equal(1, exitCode);
    }

    private async Task<int> RunMigrationServiceAsync(string? connectionString = null)
    {
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:meishi-db"] = connectionString ?? _postgres.GetConnectionString()
        });
        builder.Services.AddMigrationService(builder.Configuration);

        using var host = builder.Build();
        using var timeout = new CancellationTokenSource(Timeout);

        // The migrator reports failures through the process exit code, so reset it before each run.
        Environment.ExitCode = 0;
        await host.RunAsync(timeout.Token);

        Assert.False(timeout.IsCancellationRequested, "The migration service did not stop on its own.");

        // Reset it afterwards as well, so a failed run does not fail the test process itself
        var exitCode = Environment.ExitCode;
        Environment.ExitCode = 0;
        return exitCode;
    }

    private async Task AssertAllMigrationsAppliedAsync()
    {
        var services = new ServiceCollection();
        services.AddPostgresql(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:meishi-db"] = _postgres.GetConnectionString()
            })
            .Build());

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<MeishiContext>();

        var pending = await context.Database.GetPendingMigrationsAsync();
        var applied = await context.Database.GetAppliedMigrationsAsync();

        Assert.Empty(pending);
        Assert.Equal(context.Database.GetMigrations(), applied);
    }
}

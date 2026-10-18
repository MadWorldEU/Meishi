using Microsoft.EntityFrameworkCore;

namespace MadWorldEU.Meishi;

/// <summary>
/// Applies the pending EF Core migrations and then stops the application.
/// Aspire waits for this service to finish before it starts the API.
/// </summary>
public sealed class DatabaseMigrator(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime,
    ILogger<DatabaseMigrator> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<MeishiContext>();

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync(stoppingToken);
            logger.LogInformation("Database migrations applied.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Applying database migrations failed.");

            // A non-zero exit code makes Aspire treat the migration as failed, so the API is not started.
            Environment.ExitCode = 1;
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
}

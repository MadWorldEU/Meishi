using MadWorldEU.Meishi.CurriculaVitae;
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
    public async Task Run_WhenDatabaseIsEmpty_SeedsDefaultCurriculumVitae()
    {
        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await using var context = CreateContext();
        var curriculumVitae = Assert.Single(await context.CurriculaVitae.ToListAsync());
        Assert.True(curriculumVitae.IsDefault);
    }

    [Fact]
    public async Task Run_WhenCurriculumVitaeAlreadyExists_DoesNotSeedAgain()
    {
        // Arrange
        await RunMigrationServiceAsync();

        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await using var context = CreateContext();
        Assert.Equal(1, await context.CurriculaVitae.CountAsync());
    }

    [Fact]
    public async Task Run_WhenDatabaseIsEmpty_SeedsAllSectionsOfTheCurriculumVitae()
    {
        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await using var context = CreateContext();
        var curriculumVitae = await context.CurriculaVitae
            .Include(cv => cv.Experience)
            .Include(cv => cv.Education)
            .Include(cv => cv.SkillGroups)
            .AsSplitQuery()
            .SingleAsync();

        Assert.Equal("John", curriculumVitae.FirstName);
        Assert.Equal("Doe", curriculumVitae.LastName);

        Assert.Equal(2, curriculumVitae.Experience.Count);
        Assert.All(curriculumVitae.Experience, experience => Assert.NotEmpty(experience.Highlights));
        Assert.Single(curriculumVitae.Experience, experience => experience.Period.IsOngoing);

        var education = Assert.Single(curriculumVitae.Education);
        Assert.Equal(new Period(2016, 2020), education.Period);

        Assert.Equal(["backend", "devops", "frontend"], curriculumVitae.SkillGroups.Select(group => group.Name).Order());
        Assert.All(curriculumVitae.SkillGroups, group => Assert.NotEmpty(group.Skills));

        Assert.Equal(["email", "github", "linkedin"], curriculumVitae.ContactLinks.Select(link => link.Label));

        Assert.Equal(
            [new Language("Dutch", LanguageLevel.MotherTongue), new Language("English", LanguageLevel.Expert), new Language("German", LanguageLevel.Beginner)],
            curriculumVitae.Languages);
    }

    [Fact]
    public async Task Run_WhenOwnCurriculumVitaeExists_KeepsItAndDoesNotSeed()
    {
        // Arrange
        var ownCurriculumVitae = new CurriculumVitae(
            Guid.CreateVersion7(),
            firstName: "Jane",
            lastName: "Smith",
            role: "Architect",
            location: "Belgium",
            about: "My own curriculum vitae.",
            isDefault: false);

        await using (var context = CreateContext())
        {
            await context.Database.MigrateAsync();
            context.CurriculaVitae.Add(ownCurriculumVitae);
            await context.SaveChangesAsync();
        }

        // Act
        var exitCode = await RunMigrationServiceAsync();

        // Assert
        Assert.Equal(0, exitCode);
        await using var assertContext = CreateContext();
        var curriculumVitae = Assert.Single(await assertContext.CurriculaVitae.ToListAsync());
        Assert.Equal(ownCurriculumVitae.Id, curriculumVitae.Id);
        Assert.Equal("Jane", curriculumVitae.FirstName);
        Assert.False(curriculumVitae.IsDefault);
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
        await using var context = CreateContext();

        var pending = await context.Database.GetPendingMigrationsAsync();
        var applied = await context.Database.GetAppliedMigrationsAsync();

        Assert.Empty(pending);
        Assert.Equal(context.Database.GetMigrations(), applied);
    }

    private MeishiContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MeishiContext>()
            .UseNpgsql(_postgres.GetConnectionString(), builder =>
            {
                builder.MigrationsAssembly("Postgresql");
                builder.UseNodaTime();
            })
            .Options;

        return new MeishiContext(options);
    }
}

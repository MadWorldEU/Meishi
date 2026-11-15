using System.Net;
using System.Net.Http.Json;
using MadWorldEU.Meishi.CurriculaVitae;
using MadWorldEU.Meishi.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MadWorldEU.Meishi.Endpoints;

public sealed class CurriculaVitaeEndpointsTests(WebApplicationFactory<Program> factory, MigratedDatabaseFixture database)
    : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MigratedDatabaseFixture>
{
    [Fact]
    public async Task GetDefault_WhenCalled_ReturnsCurriculumVitae()
    {
        // Arrange
        using var client = factory
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:meishi-db", database.ConnectionString))
            .CreateClient();

        // Act
        using var response = await client.GetAsync("/curricula-vitae");
        var body = await response.Content.ReadFromJsonAsync<GetDefaultCurriculumVitaeResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.FirstName));
        Assert.False(string.IsNullOrWhiteSpace(body.LastName));
        Assert.False(string.IsNullOrWhiteSpace(body.Role));
        Assert.False(string.IsNullOrWhiteSpace(body.Location));
        Assert.False(string.IsNullOrWhiteSpace(body.About));
        Assert.All(body.Experience, experience => Assert.NotEmpty(experience.Highlights));
        Assert.NotEmpty(body.Experience);
        Assert.NotEmpty(body.Education);
        Assert.All(body.Skills, skillGroup => Assert.All(skillGroup.Skills, skill => Assert.InRange(skill.Level, 0, 10)));
        Assert.NotEmpty(body.Skills);
        Assert.NotEmpty(body.Contact);
    }

    [Fact]
    public async Task GetDefault_WhenCalled_ReturnsExperienceFromCurrentToOldest()
    {
        // Arrange
        using var client = factory
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:meishi-db", database.ConnectionString))
            .CreateClient();

        // Act
        var body = await client.GetFromJsonAsync<GetDefaultCurriculumVitaeResponse>("/curricula-vitae");

        // Assert
        Assert.NotNull(body);
        var periods = body.Experience.Select(experience => ParsePeriod(experience.Period)).ToList();
        var expected = periods.OrderByDescending(period => period.EndYear).ThenByDescending(period => period.StartYear);
        Assert.Equal(expected, periods);
    }

    /// <summary>Parses <c>2020 - 2023</c> or <c>2023 - present</c>, treating an ongoing period as ending last.</summary>
    private static (int EndYear, int StartYear) ParsePeriod(string period)
    {
        var years = period.Split(" - ");
        var endYear = years[1] == "present" ? int.MaxValue : int.Parse(years[1]);

        return (endYear, int.Parse(years[0]));
    }
}

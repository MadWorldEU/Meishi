using System.Net;
using System.Net.Http.Json;
using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MadWorldEU.Meishi.Endpoints;

public sealed class CurriculaVitaeEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetDefault_WhenCalled_ReturnsCurriculumVitae()
    {
        // Arrange
        using var client = factory.CreateClient();

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
}

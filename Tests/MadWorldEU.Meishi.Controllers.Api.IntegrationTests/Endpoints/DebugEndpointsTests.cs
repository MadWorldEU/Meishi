using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MadWorldEU.Meishi.Endpoints;

public sealed class DebugEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Ping_WhenCalled_ReturnsPong()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        using var response = await client.GetAsync("/debug/ping");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("pong", body);
    }
}

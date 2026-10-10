using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MadWorldEU.Meishi;

public sealed class CorsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string PortalOrigin = "https://portal.test";

    [Fact]
    public async Task Request_WhenOriginIsAllowed_ReturnsAllowOriginHeader()
    {
        // Arrange
        using var client = CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/debug/ping");
        request.Headers.Add("Origin", PortalOrigin);

        // Act
        using var response = await client.SendAsync(request);

        // Assert
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Equal(PortalOrigin, Assert.Single(origins));
    }

    [Fact]
    public async Task Request_WhenOriginIsNotAllowed_ReturnsNoAllowOriginHeader()
    {
        // Arrange
        using var client = CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/debug/ping");
        request.Headers.Add("Origin", "https://other.test");

        // Act
        using var response = await client.SendAsync(request);

        // Assert
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    private HttpClient CreateClient() =>
        factory
            .WithWebHostBuilder(builder => builder.UseSetting("Cors:AllowedOrigins:0", PortalOrigin))
            .CreateClient();
}

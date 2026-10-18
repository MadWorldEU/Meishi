using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace MadWorldEU.Meishi.Endpoints;

public sealed class DebugDatabaseEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17").Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    [Fact]
    public async Task Database_WhenDatabaseIsOnline_ReturnsOnline()
    {
        // Arrange
        using var client = CreateClient(_postgres.GetConnectionString());

        // Act
        using var response = await client.GetAsync("/debug/database");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("online", body);
    }

    [Fact]
    public async Task Database_WhenDatabaseIsOffline_ReturnsServiceUnavailable()
    {
        // Arrange
        // Read the connection string first, a stopped container no longer has a mapped port
        var connectionString = _postgres.GetConnectionString();
        await _postgres.StopAsync();
        using var client = CreateClient(connectionString);

        // Act
        using var response = await client.GetAsync("/debug/database");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("offline", body);
    }

    private HttpClient CreateClient(string connectionString) =>
        factory
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:meishi-db", connectionString))
            .CreateClient();
}

namespace MadWorldEU.Meishi.Endpoints;

internal static class DebugEndpoints
{
    extension(WebApplication app)
    {
        /// <summary>Maps the debug endpoints under <c>/debug</c>.</summary>
        internal void AddDebugEndpoints()
        {
            var debugEndpoints = app.MapGroup("/debug")
                .WithTags("Debug");
            
            debugEndpoints.MapGet("/ping", () => "pong");

            // Returns 200 when the API can connect to the database, and 503 when it cannot
            debugEndpoints.MapGet("/database", async (MeishiContext context, CancellationToken cancellationToken) =>
                await context.Database.CanConnectAsync(cancellationToken)
                    ? Results.Text("online")
                    : Results.Text("offline", statusCode: StatusCodes.Status503ServiceUnavailable));
        }
    }
}

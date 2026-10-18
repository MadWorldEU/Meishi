namespace MadWorldEU.Meishi.Endpoints;

public static class DebugEndpoints
{
    extension(WebApplication app)
    {
        public void AddDebugEndpoints()
        {
            var debugEndpoints = app.MapGroup("/debug");
            
            debugEndpoints.MapGet("/ping", () => "pong");

            // Returns 200 when the API can connect to the database, and 503 when it cannot
            debugEndpoints.MapGet("/database", async (MeishiContext context, CancellationToken cancellationToken) =>
                await context.Database.CanConnectAsync(cancellationToken)
                    ? Results.Text("online")
                    : Results.Text("offline", statusCode: StatusCodes.Status503ServiceUnavailable));
        }
    }
}

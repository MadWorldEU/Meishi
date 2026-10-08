namespace MadWorldEU.Meishi.Endpoints;

public static class DebugEndpoints
{
    extension(WebApplication app)
    {
        public void AddDebugEndpoints()
        {
            var debugEndpoints = app.MapGroup("/debug");
            
            debugEndpoints.MapGet("/ping", () => "pong");
        }
    }
}
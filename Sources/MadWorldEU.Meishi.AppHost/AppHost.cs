using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Api>(nameof(Api));

// Angular CLI is not Vite-driven: run `npm start` (ng serve) and hand it the Aspire-assigned port.
var portal = builder.AddJavaScriptApp("Portal", "../MadWorldEU.Meishi.Controllers.Portal", "start")
    .WithHttpEndpoint(env: "PORT");

portal
    .WithArgs(context =>
    {
        context.Args.Add("--");
        context.Args.Add("--port");
        context.Args.Add(portal.GetEndpoint("http").Property(EndpointProperty.TargetPort));
    })
    .WithReference(api)
    .WaitFor(api)
    .WithExternalHttpEndpoints();

await builder.Build().RunAsync();

using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var meishiDb = postgres.AddDatabase("meishi-db");

var api = builder.AddProject<Api>(nameof(Api))
    .WithReference(meishiDb);

// Applies the EF Core migrations and exits; the API only starts after it finished successfully.
// Shown as a child of the API in the Aspire dashboard.
var migrations = builder.AddProject<MigrationService>(nameof(MigrationService))
    .WithReference(meishiDb)
    .WaitFor(meishiDb)
    .WithParentRelationship(api);

api.WaitForCompletion(migrations);

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

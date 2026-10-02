using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Api>(nameof(Api));

builder.Build().Run();
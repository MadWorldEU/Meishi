using MadWorldEU.Meishi.Endpoints;
using MadWorldEU.Meishi.Extensions;
using Mediator;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// The Portal runs on another origin than the API, so its origins are allowed through configuration.
builder.Services.AddCors();
builder.Services.AddOptions<CorsOptions>().Configure<IConfiguration>((options, configuration) =>
{
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
});

// The ingress (Traefik) terminates HTTPS and forwards plain HTTP. X-Forwarded-Proto restores the original scheme,
// so generated URLs (such as the server URL in the OpenAPI document) use https.
// The proxy's address inside the cluster is not fixed, so every proxy is trusted; the API is only reachable through the ingress.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddPostgresql(builder.Configuration);

// The source generator finds messages and handlers in this project and in every referenced project that uses Mediator.
builder.Services.AddMediator((MediatorOptions options) =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.GenerateTypesAsInternal = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// First, so everything after it sees the original scheme.
app.UseForwardedHeaders();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors();

app.AddCurriculaVitaeEndpoints();
app.AddDebugEndpoints();

await app.RunAsync();

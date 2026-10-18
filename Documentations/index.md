---
_layout: landing
---

# Meishi

Meishi (名刺) is my digital business card. It is the personal portfolio and CV of a software developer: who I am, what I do, and the projects I've built. Offered with both hands, just like in Japan.

This site holds the technical documentation for the Meishi project.

## Project overview

Meishi is built with .NET 10 and Angular 22, stores its data in PostgreSQL, and is orchestrated with .NET Aspire.

| Project                                              | Description                                                                        |
| ---------------------------------------------------- | ---------------------------------------------------------------------------------- |
| `MadWorldEU.Meishi.AppHost`                          | Aspire AppHost that runs PostgreSQL, the migrations, the API and the portal.       |
| `MadWorldEU.Meishi.Controllers.Api`                  | ASP.NET Core Web API that serves the Meishi backend.                               |
| `MadWorldEU.Meishi.Controllers.MigrationService`     | Worker that applies the EF Core migrations on startup, before the API starts.      |
| `MadWorldEU.Meishi.Controllers.Portal`               | Angular frontend of Meishi, served by nginx in production.                         |
| `MadWorldEU.Meishi.Core.Application`                 | Application layer with the use cases that the API calls.                           |
| `MadWorldEU.Meishi.Core.Contracts`                   | Request and response models shared between the API and its clients.                |
| `MadWorldEU.Meishi.Core.Domain`                      | Domain layer with the core entities and business rules.                            |
| `MadWorldEU.Meishi.Infrastructures.Database`         | Database layer with the EF Core `MeishiContext`.                                   |
| `MadWorldEU.Meishi.Infrastructures.Postgresql`       | PostgreSQL setup for the `MeishiContext` with Npgsql, plus the EF Core migrations. |
| `MadWorldEU.Meishi.Controllers.Api.IntegrationTests` | xUnit integration tests that run the API in memory with `WebApplicationFactory`.   |
| `Deployments`                                        | Helm chart for deploying Meishi to the VPS with Kubernetes.                        |
| `Documentations`                                     | DocFx project that generates this documentation site.                              |
| `Github`                                             | GitHub Actions workflows, issue and pull request templates.                        |

## Documentation

- [Developer Guides](DeveloperGuides/DocFx.md): guides for working on the project, from setting up a development environment and releasing versions to managing the database and its migrations, configuring GitHub, the VPS, DNS and Kubernetes, and building this documentation with DocFx.

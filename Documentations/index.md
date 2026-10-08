---
_layout: landing
---

# Meishi

Meishi (名刺) is my digital business card. It is the personal portfolio and CV of a software developer: who I am, what I do, and the projects I've built. Offered with both hands, just like in Japan.

This site holds the technical documentation for the Meishi project.

## Project overview

Meishi is built with .NET 10 and Angular 22, and orchestrated with .NET Aspire.

| Project                                | Description                                                           |
| -------------------------------------- | --------------------------------------------------------------------- |
| `MadWorldEU.Meishi.AppHost`            | Aspire AppHost that wires up and runs the API and the portal locally. |
| `MadWorldEU.Meishi.Controllers.Api`    | ASP.NET Core Web API that serves the Meishi backend.                  |
| `MadWorldEU.Meishi.Controllers.Portal` | Angular frontend of Meishi, served by nginx in production.            |
| `MadWorldEU.Meishi.Core.Application`   | Application layer with the use cases that the API calls.              |
| `MadWorldEU.Meishi.Core.Contracts`     | Request and response models shared between the API and its clients.   |
| `MadWorldEU.Meishi.Core.Domain`        | Domain layer with the core entities and business rules.               |
| `Deployments`                          | Helm chart for deploying Meishi to the VPS with Kubernetes.           |
| `Documentations`                       | DocFx project that generates this documentation site.                 |
| `Github`                               | GitHub Actions workflows, issue and pull request templates.           |

## Documentation

- [Developer Guides](DeveloperGuides/DocFx.md): guides for working on the project, such as building this documentation with DocFx.

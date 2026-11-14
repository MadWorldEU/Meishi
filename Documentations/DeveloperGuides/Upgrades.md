# Upgrades
This guide explains how to keep the tools and packages of Meishi up to date:
- [.NET](#upgrade-net): upgrade the SDK and target framework to a new major version.
- [NuGet packages](#update-nuget-packages): update the packages of the API, AppHost and tests.
- [Node.js](#upgrade-nodejs): upgrade the Node.js version of the Portal.
- [npm packages](#update-npm-packages): update the packages of the Portal.

Do each upgrade on its own branch, for example `maintenance/<issue>-package-upgrade`, so a failing upgrade is easy to find and revert.

> [!IMPORTANT]
> `TreatWarningsAsErrors` is `true` in `Directory.Build.props`. NuGet warnings, such as an unneeded package reference, therefore fail the restore. Fix the warning instead of suppressing it.

## Upgrade .NET
A new major version of .NET is released every November. Even versions (such as 10) are LTS releases, odd versions (such as 11) are STS releases. See the [.NET support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core).

1. Install the new [.NET SDK](https://dotnet.microsoft.com/download/dotnet) and check the version:

```bash
dotnet --list-sdks
```

2. Change the target framework in `Directory.Build.props`. All projects inherit it:

```xml
<TargetFramework>net11.0</TargetFramework>
```

3. Change the base images in the Dockerfiles of the .NET projects:

| File | Images |
|------|--------|
| `Sources/MadWorldEU.Meishi.Controllers.Api/Dockerfile` | `mcr.microsoft.com/dotnet/aspnet` and `mcr.microsoft.com/dotnet/sdk` |
| `Sources/MadWorldEU.Meishi.Controllers.MigrationService/Dockerfile` | `mcr.microsoft.com/dotnet/runtime` and `mcr.microsoft.com/dotnet/sdk` |

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
```

4. Change `dotnet-version` in the GitHub workflows `build-and-test.yml`, `generate-docs.yml` and `sonarcube.yml` in `.github/workflows`:

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@...
  with:
    dotnet-version: 11.0.x
```

5. Update the Microsoft packages to the matching major version. See [Update NuGet Packages](#update-nuget-packages).

6. Update the `dotnet-ef` tool, so it matches the EF Core version:

```bash
dotnet tool update --global dotnet-ef
```

7. Update the .NET version in the [Setup Development Environment](SetupDevEnvironment.md) guide.

8. Check that the solution still builds and the tests pass. See [Check the Upgrade](#check-the-upgrade).

## Update NuGet Packages
The solution uses [Central Package Management](https://learn.microsoft.com/nuget/consume-packages/central-package-management). The versions of all packages are in `Directory.Packages.props` in the root of the repository. The projects only reference a package by name, without a version:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
```

1. List the packages that have a newer version, from the root of the repository:

```bash
dotnet list Meishi.slnx package --outdated
```

Add `--include-prerelease` to include preview and release candidate versions.

2. Change the versions in `Directory.Packages.props`. You can do this by hand, with the NuGet window of your IDE, or per package with:

```bash
dotnet add Sources/MadWorldEU.Meishi.Infrastructures.Postgresql package Npgsql.EntityFrameworkCore.PostgreSQL --version 11.0.0
```

With Central Package Management, `dotnet add package` changes the version in `Directory.Packages.props`, not in the project.

3. Update packages that depend on each other together, to the same version:

| Group | Packages |
|-------|----------|
| ASP.NET Core | `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.AspNetCore.OpenApi` |
| EF Core | `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Relational` |
| Npgsql | `Npgsql.EntityFrameworkCore.PostgreSQL`, `Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime` |
| Aspire | `Aspire.Hosting.*` and the `Aspire.AppHost.Sdk` version in `Sources/MadWorldEU.Meishi.AppHost/AppHost.csproj` |
| Mediator | `Mediator.Abstractions`, `Mediator.SourceGenerator` |

4. When you update Aspire, also update the Aspire CLI to the same version as `Aspire.AppHost.Sdk`:

```bash
aspire update --self
aspire --version
```

5. Restore the packages and check that there are no errors or warnings:

```bash
dotnet restore Meishi.slnx
```

6. After an EF Core or Npgsql update, check that the model has no changes that need a new migration. Run this from `Sources/MadWorldEU.Meishi.Controllers.Api` (see [Database](Database.md)):

```bash
dotnet ef migrations has-pending-model-changes --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql
```

7. Check that the solution still builds and the tests pass. See [Check the Upgrade](#check-the-upgrade).

### Common Restore Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `NU1605` / `NU1107` Detected package downgrade or version conflict | Two packages need different versions of the same dependency. For example, Npgsql `11.0.0-rc.3` needs exactly EF Core `11.0.0-rc.3`, while EF Core is on `11.0.0`. | Update the packages of the group together. When the provider has no stable version yet, keep EF Core on the version the provider needs. |
| `NU1510` PackageReference will not be pruned | The package is part of the framework, so the reference is not needed. | Remove the `PackageReference` from the project, and remove the `PackageVersion` from `Directory.Packages.props` when no project uses it anymore. |
| `NU1901`-`NU1904` Package has a known vulnerability | The package version has a security advisory. | Update the package to a version without the vulnerability. |

## Upgrade Node.js
The Portal is built with Node.js. Use an LTS version: even versions (such as 24 and 26) become LTS, odd versions (such as 27) never do. See the [Node.js releases](https://nodejs.org/about/previous-releases).

1. Check which Node.js versions the Angular version of the Portal supports:

```bash
cd Sources/MadWorldEU.Meishi.Controllers.Portal
node -p "require('./node_modules/@angular/cli/package.json').engines.node"
```

The [Angular version compatibility](https://angular.dev/reference/versions) page lists the same.

2. Install the new version and check it. With [nvm](https://github.com/nvm-sh/nvm):

```bash
nvm install 26
nvm alias default 26
node --version
```

On Linux, update the symbolic links afterwards. See [Linux: Make npm Available to Aspire](SetupDevEnvironment.md#linux-make-npm-available-to-aspire).

3. Change the Node.js version in these files. Use the same exact version everywhere, for example `26.13.1`, and check that the [Docker image tag](https://hub.docker.com/_/node) exists:

| File | Setting |
|------|---------|
| `Sources/MadWorldEU.Meishi.Controllers.Portal/Dockerfile` | `FROM node:26.13.1-alpine AS build` |
| `.github/workflows/build-and-test.yml` | `node-version: 26.13.1` |
| `.github/workflows/sonarcube.yml` | `node-version: 26.13.1` |

4. Reinstall the packages, so they are built for the new Node.js version:

```bash
npm ci
```

5. Update the Node.js version in the [Setup Development Environment](SetupDevEnvironment.md) guide.

6. Check that the Portal still builds and the tests pass. See [Check the Upgrade](#check-the-upgrade).

## Update npm Packages
Run all commands from `Sources/MadWorldEU.Meishi.Controllers.Portal`. Always commit `package.json` and `package-lock.json` together.

1. List the packages that have a newer version:

```bash
npm outdated
```

`Wanted` is the newest version that the range in `package.json` allows, `Latest` is the newest version that is published.

2. Update the Angular packages with the Angular CLI. `ng update` also runs migrations that change your code and configuration for breaking changes. It only upgrades one major version at a time, so repeat it for every major version:

```bash
npx ng update                                # List the Angular packages that can be updated
npx ng update @angular/core @angular/cli     # Update Angular
```

The [Angular update guide](https://angular.dev/update-guide) lists the manual steps for each major version.

3. Update the other packages to the newest version within their range:

```bash
npm update
```

4. Update a package to a new major version. Read its changelog first for breaking changes:

```bash
npm install <package>@latest
```

> [!WARNING]
> Do not update `typescript` beyond the range that Angular supports, and do not update `@angular/*` packages with `npm install`. Use `ng update`, otherwise the Angular packages get different versions.

5. Check for known vulnerabilities:

```bash
npm audit
```

6. Check that the Portal still builds and the tests pass. See [Check the Upgrade](#check-the-upgrade).

### Peer Dependency Errors
`npm` shows an `ERESOLVE` error when a package needs a version of another package that does not match. Update the packages that belong together, for example all `@angular/*` packages, to the same version. Do not use `--force` or `--legacy-peer-deps`: they hide the error, but the packages still do not match.

## Check the Upgrade
Run these checks before you create a pull request.

1. Build and test the API, from the root of the repository:

```bash
dotnet build Meishi.slnx
dotnet test Meishi.slnx
```

The integration tests start PostgreSQL with Testcontainers, so Docker must be running.

2. Build and test the Portal, from `Sources/MadWorldEU.Meishi.Controllers.Portal`:

```bash
npm ci
npm run build
npm test
```

3. Start the AppHost and check in the Aspire dashboard that the MigrationService finishes and that the API and Portal start:

```bash
dotnet run --project Sources/MadWorldEU.Meishi.AppHost
```

4. Build the container images, from the root of the repository:

```bash
docker build -f Sources/MadWorldEU.Meishi.Controllers.Api/Dockerfile .
docker build -f Sources/MadWorldEU.Meishi.Controllers.MigrationService/Dockerfile .
docker build -f Sources/MadWorldEU.Meishi.Controllers.Portal/Dockerfile .
```

5. After you push, check that the GitHub workflows pass.

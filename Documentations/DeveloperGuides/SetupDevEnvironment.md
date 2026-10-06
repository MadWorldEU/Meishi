# Setup Development Environment
Meishi has two applications:
- **API**: an ASP.NET Core API in `Sources/MadWorldEU.Meishi.Controllers.Api`. It runs locally through the .NET Aspire AppHost in `Sources/MadWorldEU.Meishi.AppHost`.
- **Portal**: an Angular application in `Sources/MadWorldEU.Meishi.Controllers.Portal`.

This guide explains what you need to install to build and run both projects on your own machine.

## Prerequisites

| Tool                                                          | Version                                 | Needed for                                      |
|---------------------------------------------------------------|-----------------------------------------|-------------------------------------------------|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0                                    | API and AppHost                                 |
| [Node.js](https://nodejs.org/)                                | 22.22.3+, 24.15.0+ or 26+               | Portal                                          |
| npm                                                           | Comes with Node.js                      | Portal                                          |
| [Docker](https://docs.docker.com/get-started/get-docker/)     | Any recent version                      | Optional, only to build the API container image |
| [kubectl](https://kubernetes.io/docs/tasks/tools/)            | Within one minor version of the cluster | Deployments                                     |
| [Helm](https://helm.sh/docs/intro/install/)                   | Any recent version                      | Deployments                                     |

An IDE such as JetBrains Rider, Visual Studio or Visual Studio Code is recommended. For Visual Studio Code, install the recommended [Angular Language Service](https://marketplace.visualstudio.com/items?itemName=Angular.ng-template) extension for the Portal.

## API

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and check the version:

```bash
dotnet --version
```

2. Trust the ASP.NET Core development certificate, so the `https` profiles work:

```bash
dotnet dev-certs https --trust
```

> [!NOTE]
> On Linux, `dotnet dev-certs https --trust` does not fully trust the certificate for all browsers. Follow [Linux: Setup Dev HTTPS Certificates](#linux-setup-dev-https-certificates) instead.

3. Restore the packages from the root of the repository:

```bash
dotnet restore Meishi.slnx
```

4. Start the AppHost. This starts the API and the Aspire dashboard:

```bash
dotnet run --project Sources/MadWorldEU.Meishi.AppHost
```

5. The Aspire dashboard opens at https://localhost:17268. From there you can open the API, view its logs and traces.

> [!NOTE]
> Aspire does not need a separate workload. The AppHost uses the `Aspire.AppHost.Sdk`, which is restored from NuGet.

To run the API without Aspire, start the project directly. The API is then available at http://localhost:5005:

```bash
dotnet run --project Sources/MadWorldEU.Meishi.Controllers.Api
```

### Linux: Setup Dev HTTPS Certificates

This is a one-time step, only needed on Linux (tested on Ubuntu 25.04). Install the `linux-dev-certs` tool and create locally-trusted development certificates:

```bash
dotnet tool update -g linux-dev-certs
dotnet linux-dev-certs install
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

This installs a locally-trusted root certificate, so the HTTPS endpoints (e.g. the Aspire dashboard at https://localhost:17268) are trusted by your browser without certificate warnings.

### Linux: Give Docker User Rights

Add your user to the `docker` group so you can run Docker commands without `sudo`:

```bash
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker
sudo systemctl restart docker
```

After running these commands, log out and back in (or restart your shell) for the group membership to take full effect.

## Portal

1. Install Node.js. Angular 22 supports Node.js `^22.22.3`, `^24.15.0` or `>=26.0.0`. With [nvm](https://github.com/nvm-sh/nvm) you can install it with:

```bash
nvm install 24
nvm use 24
```

> [!NOTE]
> The AppHost starts the Portal with `npm`. When Node.js is installed with nvm on Linux, follow [Linux: Make npm Available to Aspire](#linux-make-npm-available-to-aspire).

2. Go to the Portal folder and install the packages:

```bash
cd Sources/MadWorldEU.Meishi.Controllers.Portal
npm ci
```

3. Start the development server:

```bash
npm start
```

4. Open your browser and navigate to http://localhost:4200. The page reloads automatically when you change a source file.

The Angular CLI is installed as a local dependency, so a global install is not required. Use `npx ng <command>` to run Angular CLI commands, for example `npx ng generate component my-component`.

### Other commands

| Command | Description |
|---------|-------------|
| `npm run build` | Builds the Portal into the `dist/` folder |
| `npm test` | Runs the unit tests with Vitest |
| `npm run watch` | Builds the Portal in development mode and rebuilds on changes |

### Linux: Make npm Available to Aspire

nvm only adds Node.js to the `PATH` of a terminal that loads it. An IDE started from the desktop does not, so the AppHost cannot find `npm` to start the Portal. Create symbolic links in `~/.local/bin`, which Ubuntu adds to the `PATH` through `~/.profile`. `node` is needed as well, because `npm` runs with `#!/usr/bin/env node`:

```bash
NODE_BIN="$NVM_DIR/versions/node/$(nvm version default)/bin"
for b in node npm npx; do ln -sfn "$NODE_BIN/$b" ~/.local/bin/$b; done
```

Restart your IDE, or log out and back in, so it picks up the new `PATH`. The links point to a specific Node.js version, so run the commands again after changing the default version with `nvm alias default <version>`.

## Deployments

The Helm chart in `Deployments/VPS` deploys Meishi to Kubernetes. You only need these tools if you deploy, not to build or run the applications locally.

1. Install [kubectl](https://kubernetes.io/docs/tasks/tools/) and [Helm](https://helm.sh/docs/intro/install/). On Ubuntu you can install both with snap:

```bash
sudo snap install kubectl --classic
sudo snap install helm --classic
```

2. Check that both tools are installed:

```bash
kubectl version --client
helm version
```

3. Check that the chart is valid, from the root of the repository:

```bash
helm lint Deployments/VPS
```

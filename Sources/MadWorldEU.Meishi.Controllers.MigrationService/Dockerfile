# The migration service is a worker without HTTP endpoints, so the smaller runtime image is enough
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["Sources/MadWorldEU.Meishi.Controllers.MigrationService/MigrationService.csproj", "Sources/MadWorldEU.Meishi.Controllers.MigrationService/"]
COPY ["Sources/MadWorldEU.Meishi.Infrastructures.Postgresql/Postgresql.csproj", "Sources/MadWorldEU.Meishi.Infrastructures.Postgresql/"]
COPY ["Sources/MadWorldEU.Meishi.Infrastructures.Database/Database.csproj", "Sources/MadWorldEU.Meishi.Infrastructures.Database/"]
RUN dotnet restore "Sources/MadWorldEU.Meishi.Controllers.MigrationService/MigrationService.csproj"
COPY . .
WORKDIR "/src/Sources/MadWorldEU.Meishi.Controllers.MigrationService"
RUN dotnet build "./MigrationService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MigrationService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MadWorldEU.Meishi.MigrationService.dll"]

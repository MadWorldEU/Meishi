FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sources/MadWorldEU.Meishi.Controllers.Api/MadWorldEU.Meishi.Controllers.Api.csproj", "Sources/MadWorldEU.Meishi.Controllers.Api/"]
RUN dotnet restore "Sources/MadWorldEU.Meishi.Controllers.Api/MadWorldEU.Meishi.Controllers.Api.csproj"
COPY . .
WORKDIR "/src/Sources/MadWorldEU.Meishi.Controllers.Api"
RUN dotnet build "./MadWorldEU.Meishi.Controllers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MadWorldEU.Meishi.Controllers.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MadWorldEU.Meishi.Controllers.Api.dll"]

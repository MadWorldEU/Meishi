# Database

PostgreSQL is used as the database, accessed via EF Core with Npgsql. The `MeishiContext` is the single DbContext for the application (defined in `MadWorldEU.Meishi.Infrastructures.Database`; migrations live in `MadWorldEU.Meishi.Infrastructures.Postgresql`). Connection is configured via the `meishi-db` connection string, which Aspire overrides automatically at runtime.

## Migrations
This guide outlines how to manage database migrations using Entity Framework Core in the Meishi project.

### Pre-requisites
Ensure you have the dotnet-ef tool installed globally:
```bash
dotnet tool install --global dotnet-ef
```
If the tool is already installed, update it to the latest version:
```bash
dotnet tool update --global dotnet-ef
```

### Migrations
All commands should be run from the `Sources/MadWorldEU.Meishi.Controllers.Api` directory (the startup project).

#### Create Migration
```bash
dotnet ef migrations add <MigrationName> --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql -o ../MadWorldEU.Meishi.Infrastructures.Postgresql/Migrations
```

#### Apply Migration
To apply the created migration to the database:
```bash
dotnet ef database update --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql
```

#### Rollback
##### Listing All Migrations
To view all migrations:
```bash
dotnet ef migrations list --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql
```

##### Rolling Back to a Specific Migration
To rollback to a specific migration (e.g., InitialCreate):
```bash
dotnet ef database update InitialCreate --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql
```

##### Rolling Back All Migrations
To revert the database to its initial state (no migrations applied):
```bash
dotnet ef database update 0 --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql
```

#### Removing the Last Migration
If you need to remove the last migration (without applying it to the database):
```bash
dotnet ef migrations remove --context MeishiContext --project ../MadWorldEU.Meishi.Infrastructures.Postgresql -f
```

The `-f` (`--force`) flag skips the check that verifies the migration has not been applied to the database. Without it, the command fails if EF cannot confirm the migration is unapplied (e.g. no active database connection). Only use `-f` when you are certain the migration has not been applied.
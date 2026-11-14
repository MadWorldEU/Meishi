namespace MadWorldEU.Meishi.Extensions;

internal static class MigrationServiceExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the database, the default data seeder and the hosted service that migrates and seeds the database.</summary>
        internal void AddMigrationService(IConfiguration configuration)
        {
            services.AddPostgresql(configuration);
            services.AddScoped<DefaultCurriculumVitaeSeeder>();
            services.AddHostedService<DatabaseMigrator>();
        }
    }
}

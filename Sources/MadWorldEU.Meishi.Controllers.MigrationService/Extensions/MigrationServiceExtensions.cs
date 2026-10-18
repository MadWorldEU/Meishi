namespace MadWorldEU.Meishi.Extensions;

public static class MigrationServiceExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMigrationService(IConfiguration configuration)
        {
            services.AddPostgresql(configuration);
            services.AddHostedService<DatabaseMigrator>();
        }
    }
}

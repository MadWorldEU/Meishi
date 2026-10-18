using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldEU.Meishi.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPostgresql(IConfiguration configuration)
        {
            services.AddDbContext<MeishiContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("meishi-db"), 
                    builder =>
                    {
                        builder.MigrationsAssembly("Postgresql");
                        builder.UseNodaTime();
                    }));
        }
    }
}
using MadWorldEU.Meishi.CurriculaVitae;
using MadWorldEU.Meishi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MadWorldEU.Meishi.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the <see cref="MeishiContext"/> on PostgreSQL, using the <c>meishi-db</c> connection string, and the repositories.</summary>
        public void AddPostgresql(IConfiguration configuration)
        {
            services.AddDbContext<MeishiContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("meishi-db"), 
                    builder =>
                    {
                        builder.MigrationsAssembly("Postgresql");
                        builder.UseNodaTime();
                    }));

            services.AddScoped<ICurriculaVitaeRepository, CurriculaVitaeRepository>();
        }
    }
}
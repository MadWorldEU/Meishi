using MadWorldEU.Meishi.Configurations;
using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;

namespace MadWorldEU.Meishi;

public sealed class MeishiContext(DbContextOptions<MeishiContext> options) : DbContext(options)
{
    public DbSet<CurriculumVitae> CurriculaVitae => Set<CurriculumVitae>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CurriculumVitaeConfiguration());
        modelBuilder.ApplyConfiguration(new ExperienceConfiguration());
        modelBuilder.ApplyConfiguration(new EducationConfiguration());
        modelBuilder.ApplyConfiguration(new SkillGroupConfiguration());
    }
}

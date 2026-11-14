using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadWorldEU.Meishi.Configurations;

internal sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.ToTable("Educations");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.ComplexProperty(e => e.Period, period =>
        {
            period.Property(p => p.StartYear).IsRequired();
            period.Property(p => p.EndYear);
            period.Ignore(p => p.IsOngoing);
        });

        builder.Property(e => e.Degree).IsRequired();
        builder.Property(e => e.School).IsRequired();
    }
}

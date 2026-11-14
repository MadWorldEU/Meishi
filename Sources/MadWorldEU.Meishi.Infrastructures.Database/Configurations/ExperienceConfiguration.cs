using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadWorldEU.Meishi.Configurations;

internal sealed class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experiences");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.ComplexProperty(e => e.Period, period =>
        {
            period.Property(p => p.StartYear).IsRequired();
            period.Property(p => p.EndYear);
            period.Ignore(p => p.IsOngoing);
        });

        builder.Property(e => e.Role).IsRequired();
        builder.Property(e => e.Company).IsRequired();

        builder.PrimitiveCollection(e => e.Highlights)
            .HasField("_highlights")
            .IsRequired();
    }
}

using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadWorldEU.Meishi.Configurations;

public sealed class CurriculumVitaeConfiguration : IEntityTypeConfiguration<CurriculumVitae>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CurriculumVitae> builder)
    {
        builder.ToTable("CurriculaVitae");

        builder.HasKey(cv => cv.Id);
        builder.Property(cv => cv.Id).ValueGeneratedNever();

        builder.Property(cv => cv.FirstName).IsRequired();
        builder.Property(cv => cv.LastName).IsRequired();
        builder.Property(cv => cv.Role).IsRequired();
        builder.Property(cv => cv.Location).IsRequired();
        builder.Property(cv => cv.About).IsRequired();
        builder.Property(cv => cv.IsDefault).IsRequired();

        builder.HasMany(cv => cv.Experience)
            .WithOne()
            .HasForeignKey("CurriculumVitaeId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(cv => cv.Experience).HasField("_experience");

        builder.HasMany(cv => cv.Education)
            .WithOne()
            .HasForeignKey("CurriculumVitaeId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(cv => cv.Education).HasField("_education");

        builder.HasMany(cv => cv.SkillGroups)
            .WithOne()
            .HasForeignKey("CurriculumVitaeId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(cv => cv.SkillGroups).HasField("_skillGroups");

        // Complex collections must implement IList<T>, so the List<T> backing field is mapped instead of the read-only property
        builder.Ignore(cv => cv.ContactLinks);
        builder.ComplexCollection<List<ContactLink>, ContactLink>("_contactLinks", contactLinks =>
        {
            contactLinks.ToJson("ContactLinks");
            contactLinks.Property(cl => cl.Label).IsRequired();
            contactLinks.Property(cl => cl.Value).IsRequired();
            contactLinks.Property(cl => cl.Href).IsRequired();
        });

        builder.Ignore(cv => cv.DomainEvents);
    }
}

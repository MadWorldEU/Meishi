using MadWorldEU.Meishi.CurriculaVitae;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadWorldEU.Meishi.Configurations;

public sealed class SkillGroupConfiguration : IEntityTypeConfiguration<SkillGroup>
{
    public void Configure(EntityTypeBuilder<SkillGroup> builder)
    {
        builder.ToTable("SkillGroups");

        builder.HasKey(sg => sg.Id);
        builder.Property(sg => sg.Id).ValueGeneratedNever();

        builder.Property(sg => sg.Name).IsRequired();

        // Complex collections must implement IList<T>, so the List<T> backing field is mapped instead of the read-only property
        builder.Ignore(sg => sg.Skills);
        builder.ComplexCollection<List<Skill>, Skill>("_skills", skills =>
        {
            skills.ToJson("Skills");
            skills.Property(s => s.Name).IsRequired();
            skills.Property(s => s.Level).IsRequired();
        });
    }
}

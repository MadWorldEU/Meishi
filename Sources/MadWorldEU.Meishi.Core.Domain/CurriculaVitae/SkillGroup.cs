using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class SkillGroup : Entity<Guid>
{
    private readonly List<Skill> _skills = [];

    public string Name { get; private set; }

    public IReadOnlyList<Skill> Skills => _skills.AsReadOnly();

    private SkillGroup(Guid id, string name) : base(id)
    {
        // Used by EF Core, which sets Skills after construction
        Name = name;
    }

    internal SkillGroup(Guid id, string name, IEnumerable<Skill> skills) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skills);

        Name = name;

        foreach (var skill in skills)
        {
            ArgumentNullException.ThrowIfNull(skill);
            _skills.Add(skill);
        }
    }
}

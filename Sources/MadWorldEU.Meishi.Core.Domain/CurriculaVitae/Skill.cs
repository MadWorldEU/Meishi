using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class Skill : ValueObject
{
    public const int MinimumLevel = 0;
    public const int MaximumLevel = 10;

    public string Name { get; }
    /// <summary>Proficiency from 0 to 10.</summary>
    public int Level { get; }

    public Skill(string name, int level)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(level, MinimumLevel);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, MaximumLevel);

        Name = name;
        Level = level;
    }

    protected override IEnumerable<object?> GetEqualityComponents() => [Name, Level];
}

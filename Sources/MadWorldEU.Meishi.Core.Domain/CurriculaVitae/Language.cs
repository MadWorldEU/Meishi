using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class Language : ValueObject
{
    public string Name { get; }
    public LanguageLevel Level { get; }

    public Language(string name, LanguageLevel level)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (!Enum.IsDefined(level))
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be a defined language level.");
        }

        Name = name;
        Level = level;
    }

    protected override IEnumerable<object?> GetEqualityComponents() => [Name, Level];
}

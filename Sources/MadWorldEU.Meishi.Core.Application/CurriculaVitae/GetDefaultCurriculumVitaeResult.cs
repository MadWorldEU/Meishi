namespace MadWorldEU.Meishi.CurriculaVitae;

public record GetDefaultCurriculumVitaeResult
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Role { get; init; }
    public required string Location { get; init; }
    public required string About { get; init; }
    public required IReadOnlyList<ExperienceResult> Experience { get; init; }
    public required IReadOnlyList<EducationResult> Education { get; init; }
    public required IReadOnlyList<SkillGroupResult> Skills { get; init; }
    public required IReadOnlyList<ContactLinkResult> Contact { get; init; }
}

public record ExperienceResult
{
    public required string Period { get; init; }
    public required string Role { get; init; }
    public required string Company { get; init; }
    public required IReadOnlyList<string> Highlights { get; init; }
}

public record EducationResult
{
    public required string Period { get; init; }
    public required string Degree { get; init; }
    public required string School { get; init; }
}

public record SkillGroupResult
{
    public required string Name { get; init; }
    public required IReadOnlyList<SkillResult> Skills { get; init; }
}

public record SkillResult
{
    public required string Name { get; init; }
    /// <summary>Proficiency from 0 to 10.</summary>
    public required int Level { get; init; }
}

public record ContactLinkResult
{
    public required string Label { get; init; }
    public required string Value { get; init; }
    public required string Href { get; init; }
}

namespace MadWorldEU.Meishi.CurriculaVitae;

public record GetDefaultCurriculumVitaeResponse
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Role { get; init; }
    public required string Location { get; init; }
    public required string About { get; init; }
    public required IReadOnlyList<ExperienceResponse> Experience { get; init; }
    public required IReadOnlyList<EducationResponse> Education { get; init; }
    public required IReadOnlyList<SkillGroupResponse> Skills { get; init; }
    public required IReadOnlyList<ContactLinkResponse> Contact { get; init; }
}

public record ExperienceResponse
{
    public required string Period { get; init; }
    public required string Role { get; init; }
    public required string Company { get; init; }
    public required IReadOnlyList<string> Highlights { get; init; }
}

public record EducationResponse
{
    public required string Period { get; init; }
    public required string Degree { get; init; }
    public required string School { get; init; }
}

public record SkillGroupResponse
{
    public required string Name { get; init; }
    public required IReadOnlyList<SkillResponse> Skills { get; init; }
}

public record SkillResponse
{
    public required string Name { get; init; }
    /// <summary>Proficiency from 0 to 10.</summary>
    public required int Level { get; init; }
}

public record ContactLinkResponse
{
    public required string Label { get; init; }
    public required string Value { get; init; }
    public required string Href { get; init; }
}

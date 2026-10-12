namespace MadWorldEU.Meishi.CurriculaVitae;

public record GetDefaultCurriculumVitaeResult
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

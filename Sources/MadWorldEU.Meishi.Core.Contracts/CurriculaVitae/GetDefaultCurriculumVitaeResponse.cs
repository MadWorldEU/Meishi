namespace MadWorldEU.Meishi.CurriculaVitae;

public record GetDefaultCurriculumVitaeResponse
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
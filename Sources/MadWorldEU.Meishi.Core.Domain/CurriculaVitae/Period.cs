using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class Period : ValueObject
{
    public int StartYear { get; }
    /// <summary>The year the period ended, or <c>null</c> when it is still ongoing.</summary>
    public int? EndYear { get; }

    public bool IsOngoing => EndYear is null;

    public Period(int startYear, int? endYear)
    {
        if (endYear < startYear)
        {
            throw new ArgumentOutOfRangeException(nameof(endYear), endYear, "End year cannot be before the start year.");
        }

        StartYear = startYear;
        EndYear = endYear;
    }

    /// <summary>Formats the period as <c>2020 - 2023</c>, or <c>2023 - present</c> when it is still ongoing.</summary>
    public override string ToString() => $"{StartYear} - {EndYear?.ToString() ?? "present"}";

    protected override IEnumerable<object?> GetEqualityComponents() => [StartYear, EndYear];
}

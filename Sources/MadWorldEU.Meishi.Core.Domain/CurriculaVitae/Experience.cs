using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class Experience : Entity<Guid>
{
    private readonly List<string> _highlights = [];

    public Period Period { get; private set; }
    public string Role { get; private set; }
    public string Company { get; private set; }

    public IReadOnlyList<string> Highlights => _highlights.AsReadOnly();

    private Experience(Guid id, string role, string company) : base(id)
    {
        // Used by EF Core, which sets Period and Highlights after construction
        Period = null!;
        Role = role;
        Company = company;
    }

    internal Experience(Guid id, Period period, string role, string company, IEnumerable<string> highlights) : base(id)
    {
        ArgumentNullException.ThrowIfNull(period);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(company);
        ArgumentNullException.ThrowIfNull(highlights);

        Period = period;
        Role = role;
        Company = company;

        foreach (var highlight in highlights)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(highlight);
            _highlights.Add(highlight);
        }
    }
}

using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class Education : Entity<Guid>
{
    public Period Period { get; private set; }
    public string Degree { get; private set; }
    public string School { get; private set; }

    private Education(Guid id, string degree, string school) : base(id)
    {
        // Used by EF Core, which sets Period after construction
        Period = null!;
        Degree = degree;
        School = school;
    }

    internal Education(Guid id, Period period, string degree, string school) : base(id)
    {
        ArgumentNullException.ThrowIfNull(period);
        ArgumentException.ThrowIfNullOrWhiteSpace(degree);
        ArgumentException.ThrowIfNullOrWhiteSpace(school);

        Period = period;
        Degree = degree;
        School = school;
    }
}

namespace MadWorldEU.Meishi.CurriculaVitae;

public interface ICurriculaVitaeRepository
{
    /// <summary>Gets the most recently created curriculum vitae, or <c>null</c> when there is none.</summary>
    Task<CurriculumVitae?> GetLatestAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds the curriculum vitae when it does not exist yet, otherwise saves its changes.</summary>
    Task AddOrUpdateAsync(CurriculumVitae curriculumVitae, CancellationToken cancellationToken = default);
}

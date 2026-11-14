using Mediator;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class GetDefaultCurriculumVitaeQueryHandler(ICurriculaVitaeRepository repository)
    : IRequestHandler<GetDefaultCurriculumVitaeQuery, GetDefaultCurriculumVitaeResult?>
{
    /// <inheritdoc />
    public async ValueTask<GetDefaultCurriculumVitaeResult?> Handle(GetDefaultCurriculumVitaeQuery request, CancellationToken cancellationToken)
    {
        var curriculumVitae = await repository.GetLatestAsync(cancellationToken);
        return curriculumVitae?.ToResult();
    }
}

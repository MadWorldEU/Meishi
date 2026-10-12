using Mediator;

namespace MadWorldEU.Meishi.CurriculaVitae;

public class GetDefaultCurriculumVitaeQueryHandler : IRequestHandler<GetDefaultCurriculumVitaeQuery, GetDefaultCurriculumVitaeResult>
{
    public ValueTask<GetDefaultCurriculumVitaeResult> Handle(GetDefaultCurriculumVitaeQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(new GetDefaultCurriculumVitaeResult()
        {
            FirstName = "John",
            LastName = "Doe"
        });
    }
}
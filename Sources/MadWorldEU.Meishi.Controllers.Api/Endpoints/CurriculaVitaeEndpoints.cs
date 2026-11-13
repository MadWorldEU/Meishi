using MadWorldEU.Meishi.CurriculaVitae;
using MadWorldEU.Meishi.Mappings;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MadWorldEU.Meishi.Endpoints;

public static class CurriculaVitaeEndpoints
{
    extension(WebApplication app)
    {
        public void AddCurriculaVitaeEndpoints()
        {
            var curriculaVitaeEndpoints = app.MapGroup("/curricula-vitae")
                .WithTags("Curricula Vitae");
            
            curriculaVitaeEndpoints.MapGet("", async Task<Results<Ok<GetDefaultCurriculumVitaeResponse>, NotFound>> (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetDefaultCurriculumVitaeQuery(), cancellationToken);
                return result is null ? TypedResults.NotFound() : TypedResults.Ok(result.ToResponse());
            });
        }
    }
}

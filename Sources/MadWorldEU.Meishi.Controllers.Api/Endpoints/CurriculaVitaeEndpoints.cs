using MadWorldEU.Meishi.CurriculaVitae;
using MadWorldEU.Meishi.Mappings;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MadWorldEU.Meishi.Endpoints;

internal static class CurriculaVitaeEndpoints
{
    extension(WebApplication app)
    {
        /// <summary>Maps the curricula vitae endpoints under <c>/curricula-vitae</c>.</summary>
        internal void AddCurriculaVitaeEndpoints()
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

using MadWorldEU.Meishi.CurriculaVitae;

namespace MadWorldEU.Meishi.Mappings;

public static class CurriculaVitaeMappings
{
    extension(GetDefaultCurriculumVitaeResult result)
    {
        public GetDefaultCurriculumVitaeResponse ToResponse() => new()
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
        };
    }
}

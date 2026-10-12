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
            Role = result.Role,
            Location = result.Location,
            About = result.About,
            Experience = [.. result.Experience.Select(experience => experience.ToResponse())],
            Education = [.. result.Education.Select(education => education.ToResponse())],
            Skills = [.. result.Skills.Select(skillGroup => skillGroup.ToResponse())],
            Contact = [.. result.Contact.Select(contactLink => contactLink.ToResponse())],
        };
    }

    extension(ExperienceResult result)
    {
        private ExperienceResponse ToResponse() => new()
        {
            Period = result.Period,
            Role = result.Role,
            Company = result.Company,
            Highlights = result.Highlights,
        };
    }

    extension(EducationResult result)
    {
        private EducationResponse ToResponse() => new()
        {
            Period = result.Period,
            Degree = result.Degree,
            School = result.School,
        };
    }

    extension(SkillGroupResult result)
    {
        private SkillGroupResponse ToResponse() => new()
        {
            Name = result.Name,
            Skills = [.. result.Skills.Select(skill => skill.ToResponse())],
        };
    }

    extension(SkillResult result)
    {
        private SkillResponse ToResponse() => new()
        {
            Name = result.Name,
            Level = result.Level,
        };
    }

    extension(ContactLinkResult result)
    {
        private ContactLinkResponse ToResponse() => new()
        {
            Label = result.Label,
            Value = result.Value,
            Href = result.Href,
        };
    }
}

namespace MadWorldEU.Meishi.CurriculaVitae;

internal static class CurriculaVitaeMappings
{
    extension(CurriculumVitae curriculumVitae)
    {
        /// <summary>Maps the curriculum vitae and everything it contains to a <see cref="GetDefaultCurriculumVitaeResult"/>.</summary>
        public GetDefaultCurriculumVitaeResult ToResult() => new()
        {
            FirstName = curriculumVitae.FirstName,
            LastName = curriculumVitae.LastName,
            Role = curriculumVitae.Role,
            Location = curriculumVitae.Location,
            About = curriculumVitae.About,
            Experience = [.. curriculumVitae.Experience.Select(experience => experience.ToResult())],
            Education = [.. curriculumVitae.Education.Select(education => education.ToResult())],
            Skills = [.. curriculumVitae.SkillGroups.Select(skillGroup => skillGroup.ToResult())],
            Contact = [.. curriculumVitae.ContactLinks.Select(contactLink => contactLink.ToResult())],
        };
    }

    extension(Experience experience)
    {
        private ExperienceResult ToResult() => new()
        {
            Period = experience.Period.ToString(),
            Role = experience.Role,
            Company = experience.Company,
            Highlights = experience.Highlights,
        };
    }

    extension(Education education)
    {
        private EducationResult ToResult() => new()
        {
            Period = education.Period.ToString(),
            Degree = education.Degree,
            School = education.School,
        };
    }

    extension(SkillGroup skillGroup)
    {
        private SkillGroupResult ToResult() => new()
        {
            Name = skillGroup.Name,
            Skills = [.. skillGroup.Skills.Select(skill => skill.ToResult())],
        };
    }

    extension(Skill skill)
    {
        private SkillResult ToResult() => new()
        {
            Name = skill.Name,
            Level = skill.Level,
        };
    }

    extension(ContactLink contactLink)
    {
        private ContactLinkResult ToResult() => new()
        {
            Label = contactLink.Label,
            Value = contactLink.Value,
            Href = contactLink.Href,
        };
    }
}

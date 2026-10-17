using Mediator;

namespace MadWorldEU.Meishi.CurriculaVitae;

public class GetDefaultCurriculumVitaeQueryHandler : IRequestHandler<GetDefaultCurriculumVitaeQuery, GetDefaultCurriculumVitaeResult>
{
    public ValueTask<GetDefaultCurriculumVitaeResult> Handle(GetDefaultCurriculumVitaeQuery request, CancellationToken cancellationToken)
    {
        // Placeholder content: replace with your own details.
        return ValueTask.FromResult(new GetDefaultCurriculumVitaeResult()
        {
            FirstName = "John",
            LastName = "Doe",
            Role = "Software Engineer",
            Location = "The Netherlands",
            About = "Full-stack developer who enjoys building reliable back-ends in .NET and clean front-ends " +
                    "in Angular. Passionate about clean architecture, automated testing and shipping often.",
            Experience =
            [
                new ExperienceResult
                {
                    Period = "2023 - present",
                    Role = "Senior Software Engineer",
                    Company = "Company A",
                    Highlights =
                    [
                        "Designed and built ASP.NET Core APIs orchestrated with .NET Aspire.",
                        "Set up CI/CD pipelines with GitHub Actions and Docker.",
                    ],
                },
                new ExperienceResult
                {
                    Period = "2020 - 2023",
                    Role = "Software Engineer",
                    Company = "Company B",
                    Highlights =
                    [
                        "Developed Angular applications used by thousands of customers.",
                        "Introduced unit and integration testing into the team workflow.",
                    ],
                },
            ],
            Education =
            [
                new EducationResult
                {
                    Period = "2016 - 2020",
                    Degree = "BSc Computer Science",
                    School = "University of Applied Sciences",
                },
            ],
            Skills =
            [
                new SkillGroupResult
                {
                    Name = "backend",
                    Skills =
                    [
                        new SkillResult { Name = "C# / .NET", Level = 9 },
                        new SkillResult { Name = "ASP.NET Core", Level = 9 },
                        new SkillResult { Name = "SQL", Level = 7 },
                    ],
                },
                new SkillGroupResult
                {
                    Name = "frontend",
                    Skills =
                    [
                        new SkillResult { Name = "Angular", Level = 8 },
                        new SkillResult { Name = "TypeScript", Level = 8 },
                        new SkillResult { Name = "SCSS", Level = 7 },
                    ],
                },
                new SkillGroupResult
                {
                    Name = "devops",
                    Skills =
                    [
                        new SkillResult { Name = "Docker", Level = 7 },
                        new SkillResult { Name = "GitHub Actions", Level = 7 },
                    ],
                },
            ],
            Contact =
            [
                new ContactLinkResult { Label = "email", Value = "you@example.com", Href = "mailto:you@example.com" },
                new ContactLinkResult { Label = "github", Value = "github.com/your-name", Href = "https://github.com/your-name" },
                new ContactLinkResult
                {
                    Label = "linkedin",
                    Value = "linkedin.com/in/your-name",
                    Href = "https://www.linkedin.com/in/your-name",
                },
            ],
        });
    }
}

using MadWorldEU.Meishi.CurriculaVitae;

namespace MadWorldEU.Meishi;

/// <summary>
/// Adds a placeholder curriculum vitae when the database does not contain one yet, so the API has something to show.
/// </summary>
internal sealed class DefaultCurriculumVitaeSeeder(ICurriculaVitaeRepository repository, ILogger<DefaultCurriculumVitaeSeeder> logger)
{
    /// <summary>Adds the default curriculum vitae when the database does not contain a curriculum vitae yet.</summary>
    internal async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await repository.GetLatestAsync(cancellationToken) is not null)
        {
            logger.LogInformation("A curriculum vitae already exists, skipping the default seed.");
            return;
        }

        logger.LogInformation("Seeding the default curriculum vitae...");
        await repository.AddOrUpdateAsync(CreateDefault(), cancellationToken);
        logger.LogInformation("Default curriculum vitae seeded.");
    }

    private static CurriculumVitae CreateDefault()
    {
        // Placeholder content: replace with your own details.
        var curriculumVitae = new CurriculumVitae(
            Guid.CreateVersion7(),
            firstName: "John",
            lastName: "Doe",
            role: "Software Engineer",
            location: "The Netherlands",
            about: "Full-stack developer who enjoys building reliable back-ends in .NET and clean front-ends " +
                   "in Angular. Passionate about clean architecture, automated testing and shipping often.",
            isDefault: true);

        curriculumVitae.AddExperience(
            new Period(2023, null),
            "Senior Software Engineer",
            "Company A",
            [
                "Designed and built ASP.NET Core APIs orchestrated with .NET Aspire.",
                "Set up CI/CD pipelines with GitHub Actions and Docker.",
            ]);
        curriculumVitae.AddExperience(
            new Period(2020, 2023),
            "Software Engineer",
            "Company B",
            [
                "Developed Angular applications used by thousands of customers.",
                "Introduced unit and integration testing into the team workflow.",
            ]);

        curriculumVitae.AddEducation(new Period(2016, 2020), "BSc Computer Science", "University of Applied Sciences");

        curriculumVitae.AddSkillGroup("backend", [new Skill("C# / .NET", 9), new Skill("ASP.NET Core", 9), new Skill("SQL", 7)]);
        curriculumVitae.AddSkillGroup("frontend", [new Skill("Angular", 8), new Skill("TypeScript", 8), new Skill("SCSS", 7)]);
        curriculumVitae.AddSkillGroup("devops", [new Skill("Docker", 7), new Skill("GitHub Actions", 7)]);

        curriculumVitae.AddContactLink(new ContactLink("email", "you@example.com", "mailto:you@example.com"));
        curriculumVitae.AddContactLink(new ContactLink("github", "github.com/your-name", "https://github.com/your-name"));
        curriculumVitae.AddContactLink(new ContactLink("linkedin", "linkedin.com/in/your-name", "https://www.linkedin.com/in/your-name"));

        curriculumVitae.AddLanguage(new Language("Dutch", LanguageLevel.MotherTongue));
        curriculumVitae.AddLanguage(new Language("English", LanguageLevel.Expert));
        curriculumVitae.AddLanguage(new Language("German", LanguageLevel.Beginner));

        curriculumVitae.AddHobby("Running");
        curriculumVitae.AddHobby("Photography");
        curriculumVitae.AddHobby("Board games");

        return curriculumVitae;
    }
}

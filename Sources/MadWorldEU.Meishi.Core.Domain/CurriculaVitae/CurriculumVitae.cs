using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class CurriculumVitae : AggregateRoot<Guid>
{
    private readonly List<Experience> _experience = [];
    private readonly List<Education> _education = [];
    private readonly List<SkillGroup> _skillGroups = [];
    private readonly List<ContactLink> _contactLinks = [];
    private readonly List<Language> _languages = [];
    private readonly List<string> _hobbies = [];

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Role { get; private set; }
    public string Location { get; private set; }
    public string About { get; private set; }
    public bool IsDefault { get; private set; }

    public IReadOnlyList<Experience> Experience => _experience.AsReadOnly();
    public IReadOnlyList<Education> Education => _education.AsReadOnly();
    public IReadOnlyList<SkillGroup> SkillGroups => _skillGroups.AsReadOnly();
    public IReadOnlyList<ContactLink> ContactLinks => _contactLinks.AsReadOnly();
    public IReadOnlyList<Language> Languages => _languages.AsReadOnly();
    public IReadOnlyList<string> Hobbies => _hobbies.AsReadOnly();

    public CurriculumVitae(Guid id, string firstName, string lastName, string role, string location, string about, bool isDefault)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        ArgumentException.ThrowIfNullOrWhiteSpace(location);
        ArgumentException.ThrowIfNullOrWhiteSpace(about);

        FirstName = firstName;
        LastName = lastName;
        Role = role;
        Location = location;
        About = about;
        IsDefault = isDefault;
    }

    /// <summary>Adds a work experience to the curriculum vitae and returns it.</summary>
    public Experience AddExperience(Period period, string role, string company, IEnumerable<string> highlights)
    {
        var experience = new Experience(Guid.CreateVersion7(), period, role, company, highlights);
        _experience.Add(experience);
        return experience;
    }

    /// <summary>Adds an education to the curriculum vitae and returns it.</summary>
    public Education AddEducation(Period period, string degree, string school)
    {
        var education = new Education(Guid.CreateVersion7(), period, degree, school);
        _education.Add(education);
        return education;
    }

    /// <summary>Adds a named group of skills to the curriculum vitae and returns it.</summary>
    public SkillGroup AddSkillGroup(string name, IEnumerable<Skill> skills)
    {
        var skillGroup = new SkillGroup(Guid.CreateVersion7(), name, skills);
        _skillGroups.Add(skillGroup);
        return skillGroup;
    }

    /// <summary>Adds a way to get in touch, such as an email address or a profile link.</summary>
    public void AddContactLink(ContactLink contactLink)
    {
        ArgumentNullException.ThrowIfNull(contactLink);
        _contactLinks.Add(contactLink);
    }

    /// <summary>Adds a language the person speaks, together with how well they speak it.</summary>
    public void AddLanguage(Language language)
    {
        ArgumentNullException.ThrowIfNull(language);
        _languages.Add(language);
    }

    /// <summary>Adds something the person enjoys doing outside of work, such as running or photography.</summary>
    public void AddHobby(string hobby)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hobby);
        _hobbies.Add(hobby);
    }
}

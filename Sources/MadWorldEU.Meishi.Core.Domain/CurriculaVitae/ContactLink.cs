using MadWorldEU.Meishi.DomainDrivenDevelopment;

namespace MadWorldEU.Meishi.CurriculaVitae;

public sealed class ContactLink : ValueObject
{
    public string Label { get; }
    public string Value { get; }
    public string Href { get; }

    public ContactLink(string label, string value, string href)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(href);

        if (!Uri.TryCreate(href, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Href must be an absolute URI, such as https:// or mailto:.", nameof(href));
        }

        Label = label;
        Value = value;
        Href = href;
    }

    protected override IEnumerable<object?> GetEqualityComponents() => [Label, Value, Href];
}

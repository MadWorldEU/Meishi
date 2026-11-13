namespace MadWorldEU.Meishi.DomainDrivenDevelopment;

/// <summary>
/// Base class for aggregate roots: the entry point of an aggregate and its consistency boundary.
/// Collects domain events that are raised while the aggregate changes, so they can be dispatched after saving.
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Removes all collected domain events, typically after they have been dispatched.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}

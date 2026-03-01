namespace Bookify.Domain.Abstractions;
public abstract class Entity
{
    // Internal collection of domain events raised by this entity
    private readonly List<IDomainEvent> _domainEvents = new();

    protected Entity(Guid id)  // Creates a new entity with a unique identifier.
    {
        Id = id;
    }

    public Guid Id { get; init; }  // Unique identifier of the entity.

    /// <summary>
    /// Returns a read-only copy of the domain events.
    /// 
    /// Important: We return a copy to protect encapsulation.
    /// </summary>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    /// <summary>
    /// Clears all domain events.
    /// 
    /// Usually called after events are dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Raises a new domain event.
    /// 
    /// Should only be called from inside the entity
    /// when something meaningful happens in the domain.
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users.Events;
/// <summary>
/// Domain event raised when a new user is created.
///
/// This event represents a meaningful business occurrence
/// inside the Users aggregate.
/// 
/// It can be handled to:
/// - Send a welcome email
/// - Create a default profile
/// - Publish an integration event
/// - Log activity
/// </summary>
public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;

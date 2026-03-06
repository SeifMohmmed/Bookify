using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;
/// <summary>
/// Represents the User aggregate root.
/// Responsible for maintaining user data and assigned roles.
/// </summary>
public sealed class User : Entity
{
    private readonly List<Role> _roles = new();    // Internal collection of roles assigned to the user.
    private User(
        Guid id,
        FirstName firstName,
        LastName lastName,
        Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {

    }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }

    public string IdentityId { get; private set; } = string.Empty; // External identity provider identifier (Keycloak user id).

    /// <summary>
    /// Exposes the roles assigned to the user.
    /// Returns a read-only collection to preserve aggregate integrity.
    /// </summary>
    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    /// <summary>
    /// Factory method responsible for creating a new User aggregate.
    /// 
    /// Ensures:
    /// - Proper identity generation
    /// - Aggregate invariants enforcement
    /// - Raising the appropriate domain event
    /// - Assigning the default role (Registered)
    /// </summary>
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.CreateVersion7(), firstName, lastName, email);

        // Raise domain event to signal that a new user has been created
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        // Assign default role
        user._roles.Add(Role.Registered);

        return user;
    }

    /// <summary>
    /// Sets the identity provider identifier for the user.
    /// </summary>
    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}

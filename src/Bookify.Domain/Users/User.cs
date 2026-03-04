using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;
public sealed class User : Entity
{
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

    /// <summary>
    /// Factory method responsible for creating a new User aggregate.
    /// 
    /// Ensures:
    /// - Proper identity generation
    /// - Aggregate invariants enforcement
    /// - Raising the appropriate domain event
    /// </summary>
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.CreateVersion7(), firstName, lastName, email);

        // Raise domain event to signal that a new user has been created
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }
}

namespace Bookify.Domain.Users;
/// <summary>
/// Represents a Role in the system.
/// Roles are used to group permissions and authorize users.
/// </summary>
public sealed class Role
{
    /// <summary>
    /// Predefined role instance representing a registered user.
    /// Used as the default role when a new user is created.
    /// </summary>
    public static readonly Role Registered = new(1, "Registered");

    public Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public ICollection<User> Users { get; init; } = new List<User>();

}

namespace Bookify.Domain.Users;
/// <summary>
/// Represents a permission in the system.
/// Permissions define fine-grained access control
/// that can be assigned to roles.
/// </summary
public sealed class Permission
{
    /// Predefined permission allowing reading users.
    public static readonly Permission UsersRead = new Permission(1, "users:read");

    public Permission(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }    // Unique identifier of the permission.

    /// <summary>
    /// Permission name used for authorization checks.
    /// Example: users:read, bookings:create
    /// </summary>
    public string Name { get; init; }
}

namespace Bookify.Domain.Users;
/// <summary>
/// Join entity representing the many-to-many relationship
/// between Roles and Permissions.
/// </summary>
public class RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}

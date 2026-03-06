using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Custom authorization attribute used to require a specific permission.
/// 
/// Example:
/// [HasPermission(Permissions.UsersRead)]
/// </summary>
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(permission)
    {
    }
}
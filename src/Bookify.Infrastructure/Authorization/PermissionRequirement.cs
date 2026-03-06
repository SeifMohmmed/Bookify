using Microsoft.AspNetCore.Authorization;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Represents a permission requirement used during authorization.
/// </summary>
internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }

    public string Permission { get; }
}
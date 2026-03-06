using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Service responsible for retrieving authorization-related data
/// from the database (e.g., roles assigned to a user).
/// </summary>
internal class AuthorizationService
    (ApplicationDbContext context)
{
    /// <summary>
    /// Retrieves all roles assigned to a user based on their IdentityId.
    /// </summary>
    /// <param name="identityId">
    /// Identity provider user identifier (from Keycloak).
    /// </param>
    /// <returns>
    /// A response containing the application User Id and the list of roles.
    /// </returns>
    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        var roles =
            await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRoleResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        return roles;
    }
    /// <summary>
    /// Retrieves all permissions assigned to a user through their roles.
    /// </summary>
    /// <param name="identityId">
    /// Identity provider user identifier (e.g., from Keycloak).
    /// </param>
    /// <returns>
    /// A set of permission names assigned to the user.
    /// </returns>
    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var permissions = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            // Flatten all role permissions for the user
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        // Convert permissions to a hash set of names for fast lookup
        var permissionSet = permissions
            .Select(p => p.Name)
            .ToHashSet();

        return permissionSet;
    }
}

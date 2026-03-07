using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Service responsible for retrieving authorization-related data
/// from the database (e.g., roles assigned to a user).
/// </summary>
internal class AuthorizationService(
    ApplicationDbContext context,
    ICacheService cacheService)
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
        var cacheKey = $"auth:roles-{identityId}";

        var cachedRoles = await cacheService.GetAsync<UserRoleResponse>(cacheKey);

        if (cachedRoles is not null)
            return cachedRoles;

        var roles =
            await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRoleResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        await cacheService.SetAsync(cacheKey, roles);

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
        var cacheKey = $"auth:permissions-{identityId}";

        var cachedPermissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
            return cachedPermissions;

        var permissions = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            // Flatten all role permissions for the user
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        // Convert permissions to a hash set of names for fast lookup
        var permissionSet = permissions
            .Select(p => p.Name)
            .ToHashSet();

        await cacheService.SetAsync(cacheKey, permissionSet);

        return permissionSet;
    }
}

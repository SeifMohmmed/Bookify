using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Service responsible for retrieving authorization data such as
/// user roles and permissions from the database.
///
/// To improve performance, retrieved data is cached using Redis
/// through the <see cref="ICacheService"/> abstraction.
/// </summary>
internal class AuthorizationService(
    ApplicationDbContext context,
    ICacheService cacheService)
{
    /// <summary>
    /// Retrieves all roles assigned to a user based on their IdentityId.
    /// Results are cached to reduce repeated database queries.
    /// </summary>
    /// <param name="identityId">
    /// Unique user identifier provided by the identity provider (e.g., Keycloak).
    /// </param>
    /// <returns>
    /// A <see cref="UserRoleResponse"/> containing the application user Id
    /// and the roles assigned to the user.
    /// </returns>
    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        // Try retrieving roles from cache
        var cachedRoles = await cacheService.GetAsync<UserRoleResponse>(cacheKey);

        if (cachedRoles is not null)
            return cachedRoles;

        // Fetch roles from database if not found in cache
        var roles =
            await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRoleResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        // Store result in cache
        await cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    /// <summary>
    /// Retrieves all permissions assigned to a user through their roles.
    /// Results are cached to reduce database access.
    /// </summary>
    /// <param name="identityId">
    /// Unique identifier from the identity provider (e.g., Keycloak).
    /// </param>
    /// <returns>
    /// A set of permission names granted to the user.
    /// </returns>
    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";

        // Attempt to retrieve cached permissions
        var cachedPermissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
            return cachedPermissions;

        // Retrieve permissions from database through user roles
        var permissions = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            // Flatten all role permissions for the user
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        // Convert permission objects to permission names
        var permissionSet = permissions
            .Select(p => p.Name)
            .ToHashSet();

        // Cache permissions
        await cacheService.SetAsync(cacheKey, permissionSet);

        return permissionSet;
    }
}

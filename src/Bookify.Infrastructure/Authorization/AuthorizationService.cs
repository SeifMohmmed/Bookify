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
}

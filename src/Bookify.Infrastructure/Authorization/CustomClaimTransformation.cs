using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Adds role claims to the authenticated user based on the roles
/// stored in the application's database.
/// This runs after authentication and enriches the ClaimsPrincipal.
/// </summary>
internal sealed class CustomClaimTransformation
    (IServiceProvider serviceProvider) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(
        ClaimsPrincipal principal)
    {
        // If roles and subject already exist, skip transformation
        if (principal.HasClaim(claim => claim.Type == ClaimTypes.Role) &&
            principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub))
            return principal;

        using var scope = serviceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        // Extract IdentityId from the token
        var identityId = principal.GetIdentityId();

        // Retrieve user roles from the database
        var userRoles = await authorizationService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();

        // Add application UserId as the Subject claim
        claimsIdentity.AddClaim(new Claim(
            JwtRegisteredClaimNames.Sub,
            userRoles.Id.ToString()));

        // Add role claims
        foreach (var role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(
                ClaimTypes.Role,
                role.Name));
        }

        // Attach the new identity to the current principal
        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}

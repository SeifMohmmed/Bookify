using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Extension methods for working with ClaimsPrincipal.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Retrieves the user's identity identifier from the authentication claims.
    /// </summary>
    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ApplicationException("User identity is unavailable");
    }

    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        var userId = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
               throw new ApplicationException("User Identifier is unavailable");
    }
}
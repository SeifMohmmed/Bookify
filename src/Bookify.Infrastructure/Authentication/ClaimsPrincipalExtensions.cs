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
}
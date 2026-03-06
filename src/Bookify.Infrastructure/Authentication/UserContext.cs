using Bookify.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Provides access to the currently authenticated user's identity
/// by reading it from the HTTP context.
/// </summary>
internal sealed class UserContext(
    IHttpContextAccessor httpContextAccessor) : IUserContext
{
    /// <summary>
    /// Gets the identity ID of the currently authenticated user.
    /// Extracted from the JWT claims in the HttpContext.
    /// </summary>
    public string IdentityId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");

    public Guid UserId =>
          httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");

}
using Bookify.Domain.Abstractions;

namespace Bookify.Application.Abstractions.Authentication;
/// <summary>
/// Service responsible for requesting JWT access tokens
/// from the external Identity Provider (Keycloak).
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Requests an access token using user credentials.
    /// </summary>
    /// <returns>
    /// Result containing the JWT access token if authentication succeeds,
    /// otherwise a failure result.
    /// </returns>
    Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
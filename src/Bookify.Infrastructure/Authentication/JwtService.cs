using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Implementation of IJwtService that communicates with Keycloak
/// to obtain JWT tokens.
/// </summary>
internal sealed class JwtService : IJwtService
{
    /// <summary>
    /// Error returned when authentication with Keycloak fails.
    /// </summary>
    private static readonly Error AuthenticationFailed = new("Keycloak.AuthenticationFailed",
        "Failed to acquire access token to do authentication failure");

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Requests a JWT access token using the Resource Owner Password Credentials flow.
    /// </summary>
    public async Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Prepare form parameters required by Keycloak
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password)
            };

            // Convert parameters to form-url-encoded request
            var authorizationRequestContent =
                new FormUrlEncodedContent(authRequestParameters);

            // Send authentication request to Keycloak token endpoint
            var response = await _httpClient.PostAsync(
                "", authorizationRequestContent, cancellationToken);

            response.EnsureSuccessStatusCode();

            // Deserialize response into AuthorizationToken model
            var authorizationToken =
                await response.Content.ReadFromJsonAsync<AuthorizationToken>();

            if (authorizationToken is null)
                return Result.Failure<string>(AuthenticationFailed);

            // Return JWT access token
            return authorizationToken.AccessToken;
        }

        catch (HttpRequestException)
        {
            return Result.Failure<string>(AuthenticationFailed);
        }
    }
}
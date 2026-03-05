using System.Text.Json.Serialization;

namespace Bookify.Infrastructure.Authentication.Models;
/// <summary>
/// Represents the response returned from the Keycloak token endpoint.
/// Used to extract the access token required for authenticating requests
/// to the Keycloak Admin API.
/// </summary>
public sealed class AuthorizationToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty; // This token is used in the Authorization header (Bearer token)

}
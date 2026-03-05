using Bookify.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Delegating handler responsible for automatically attaching an admin
/// access token (JWT) to outgoing HTTP requests sent to Keycloak Admin APIs.
/// </summary>
public sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    // Holds Keycloak configuration values such as client id and secret
    private readonly KeycloakOptions _keycloakOptions;

    /// <summary>
    /// Constructor that receives Keycloak configuration via IOptions pattern.
    /// </summary>
    public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
    {
        _keycloakOptions = keycloakOptions.Value;
    }

    /// <summary>
    /// Intercepts outgoing HTTP requests and attaches the Authorization header
    /// with a valid access token before sending the request.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Obtain a new access token from Keycloak using client credentials flow
        var authorizationToken = await GetAuthorizationToken(cancellationToken);

        // Attach the JWT token to the Authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme, authorizationToken.AccessToken);

        // Send the original HTTP request with the Authorization header attached
        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        // Throw exception if response status code is not successful (2xx)
        httpResponseMessage.EnsureSuccessStatusCode();

        return httpResponseMessage;
    }

    /// <summary>
    /// Requests an access token from Keycloak using the client_credentials grant type.
    /// This token is used to authenticate admin-level API calls.
    /// </summary>
    private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        // Prepare form parameters required by Keycloak token endpoint
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            // Keycloak client id configured for admin access
            new("client_id", _keycloakOptions.AdminClientId),

            // Secret associated with the client
            new("client_secret", _keycloakOptions.AdminClientSecret),

            // Requested scopes
            new("scope", "openid email"),

            // OAuth2 grant type used here
            new("grant_type", "client_credentials")
        };

        // Convert parameters to application/x-www-form-urlencoded content
        var authorizationRequestContext = new FormUrlEncodedContent(authorizationRequestParameters);

        // Create HTTP request to Keycloak token endpoint
        var authorizationRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_keycloakOptions.TokenUrl))
        {
            Content = authorizationRequestContext
        };

        // Send token request to Keycloak
        var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

        // Ensure Keycloak returned a successful response
        authorizationResponse.EnsureSuccessStatusCode();

        // Deserialize the JSON response into AuthorizationToken object
        return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>()
               ?? throw new ApplicationException();
    }
}
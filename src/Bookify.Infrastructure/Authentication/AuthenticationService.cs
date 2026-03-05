using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication.Models;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication;
/// <summary>
/// Service responsible for communicating with Keycloak Admin API
/// to perform authentication-related operations such as user registration.
/// </summary>
internal sealed class AuthenticationService : IAuthenticationService
{
    /// <summary>
    /// Keycloak credential type used for password authentication.
    /// </summary>
    private const string PasswordCredentialType = "password";

    /// <summary>
    /// HTTP client used to send requests to Keycloak Admin API.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructor that receives a configured HttpClient.
    /// The client usually has BaseAddress and Authorization handler configured.
    /// </summary>
    public AuthenticationService(HttpClient httpClient) => _httpClient = httpClient;

    /// <summary>
    /// Registers a new user in Keycloak.
    /// </summary>
    /// <param name="user">Domain user entity</param>
    /// <param name="password">User password</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Identity provider user ID (Keycloak user ID)</returns>
    public async Task<string> RegisterAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Convert the domain User entity to Keycloak representation model
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        // Attach password credentials to the user
        userRepresentationModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                // User password
                Value = password,

                // If true, user must change password on first login
                Temporary = false,

                // Credential type required by Keycloak
                Type = PasswordCredentialType
            }
        };

        // Send request to Keycloak Admin API to create the user
        var response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentationModel,
            cancellationToken);

        // Extract the created user's ID from the response Location header
        return ExtractIdentityIdFromLocationHeader(response);
    }

    /// <summary>
    /// Extracts the user ID from the Location header returned by Keycloak.
    /// Example header:
    /// /admin/realms/bookify/users/{userId}
    /// </summary>
    private string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
    {
        const string usersSegmentName = "users/";

        // Example value:
        // /admin/realms/bookify/users/9c5f6e23-xxxx-xxxx
        var locationHeader = response.Headers.Location?.PathAndQuery;

        // Ensure the Location header exists
        if (locationHeader is null)
            throw new InvalidOperationException("Location header can't be null");

        // Find the index of "users/" inside the URL
        var userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        // Extract the substring after "users/" which represents the user ID
        return locationHeader.Substring(
            userSegmentValueIndex + usersSegmentName.Length);
    }
}
using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LoginUser;
using System.Net.Http.Json;

namespace Bookify.Api.FunctionalTests.Infrastructure;
/// <summary>
/// Base class for all functional tests.
/// Provides a configured HttpClient that communicates with the in-memory test server.
/// </summary>
public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    /// <summary>
    /// HttpClient used to send HTTP requests to the test API server.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Initializes the HttpClient using the custom WebApplicationFactory.
    /// This client communicates with the test host configured for functional tests.
    /// </summary>
    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    /// <summary>
    /// Helper method used by tests that require authentication.
    /// It logs in using the predefined test user and returns a JWT access token.
    /// </summary>
    protected async Task<string> GetAccessToken()
    {
        // Send login request to the API
        HttpResponseMessage loginResponse = await HttpClient.PostAsJsonAsync(
            "v1/users/login",
            new LogInUserRequest(
                UserData.RegisterTestUserRequest.Email,
                UserData.RegisterTestUserRequest.Password));

        // Deserialize the access token from the response body
        var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        // Return the JWT access token
        return accessTokenResponse!.AccessToken;
    }
}
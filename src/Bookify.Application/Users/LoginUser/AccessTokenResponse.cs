namespace Bookify.Application.Users.LoginUser;
/// <summary>
/// Response returned to the client after successful authentication.
/// Contains the JWT access token.
/// </summary>
public sealed record AccessTokenResponse(string AccessToken);
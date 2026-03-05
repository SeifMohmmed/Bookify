
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Users.LoginUser;

namespace Bookify.Users.LoginUser;
/// <summary>
/// Command representing a login request.
/// </summary>
public sealed record LogInUserCommand(string Email, string Password)
    : ICommand<AccessTokenResponse>;
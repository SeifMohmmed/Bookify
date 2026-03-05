using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;
using Bookify.Users.LoginUser;

namespace Bookify.Application.Users.LoginUser;
/// <summary>
/// Handles the user login command.
/// Responsible for requesting a JWT token from the authentication service.
/// </summary>
internal sealed class LogInUserCommandHandler(
    IJwtService jwtService) : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
    public async Task<Result<AccessTokenResponse>> Handle(
        LogInUserCommand request,
        CancellationToken cancellationToken)
    {
        // Request access token from identity provider
        var result = await jwtService.GetAccessTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // If authentication failed return domain error
        if (result.IsFailure)
            return Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials);

        // Return access token to API layer
        return new AccessTokenResponse(result.Value);
    }
}
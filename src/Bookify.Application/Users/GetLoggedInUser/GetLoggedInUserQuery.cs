using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Users.GetLoggedInUser;
/// <summary>
/// Query used to retrieve the currently logged-in user.
/// This query does not require parameters because the user identity
/// is retrieved from the current authentication context.
/// </summary>
public sealed record GetLoggedInUserQuery : IQuery<UserResponse>;
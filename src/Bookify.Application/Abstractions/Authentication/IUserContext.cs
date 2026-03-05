namespace Bookify.Application.Abstractions.Authentication;
/// <summary>
/// Represents the current authenticated user context.
/// Provides access to information about the logged-in user.
/// </summary>
public interface IUserContext
{
    string IdentityId { get; }
}

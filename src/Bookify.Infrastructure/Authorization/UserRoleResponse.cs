using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// DTO used to return the roles assigned to a user.
/// </summary>
public sealed class UserRoleResponse
{
    public Guid Id { get; init; }

    public List<Role> Roles { get; init; } = [];   // List of roles assigned to the user.
}

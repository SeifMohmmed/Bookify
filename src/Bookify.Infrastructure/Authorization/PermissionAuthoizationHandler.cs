using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Authorization handler responsible for validating
/// whether the current user has the required permission.
/// </summary>
internal sealed class PermissionAuthoizationHandler(
    IServiceProvider serviceProvider) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Ensure the user is authenticated
        if (context.User.Identity is not { IsAuthenticated: true })
            return;

        using var scope = serviceProvider.CreateAsyncScope();

        var authorizationService =
            scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        // Retrieve identity provider id
        var identityId = context.User.GetIdentityId();

        // Get permissions assigned to the user
        HashSet<string> permissions =
            await authorizationService.GetPermissionsForUserAsync(identityId);

        // Check if the user has the required permission
        if (permissions.Contains(requirement.Permission))
            context.Succeed(requirement);
    }
}

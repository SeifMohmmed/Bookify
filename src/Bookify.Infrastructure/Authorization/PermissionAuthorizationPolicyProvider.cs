using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authorization;
/// <summary>
/// Custom policy provider that dynamically creates
/// authorization policies for permissions.
/// </summary>
internal sealed class PermissionAuthorizationPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authoirizationOptions;
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
        _authoirizationOptions = options.Value;
    }

    /// <summary>
    /// Retrieves or dynamically creates an authorization policy
    /// based on the requested permission name.
    /// </summary>
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
            return policy;

        // Create policy dynamically based on permission name
        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _authoirizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}

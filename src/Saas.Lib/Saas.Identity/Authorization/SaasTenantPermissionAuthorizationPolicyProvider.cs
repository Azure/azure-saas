using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Saas.Identity.Authorization.Requirement;

namespace Saas.Identity.Authorization;
public class SaasTenantPermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{

    public SaasTenantPermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options) : base(options)
    {

    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }

        return new AuthorizationPolicyBuilder()
            .AddRequirements(new SaasTenantPermissionRequirement(policyName))
            .Build();
    }
}
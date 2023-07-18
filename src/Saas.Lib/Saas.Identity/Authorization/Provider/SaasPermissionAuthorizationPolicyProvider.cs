using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Saas.Identity.Authorization.Attribute;
using Saas.Identity.Authorization.Model;
using Saas.Identity.Authorization.Requirement;
using System.Reflection;

namespace Saas.Identity.Authorization.Provider;
public class SaasPermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public SaasPermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options) : base(options)
    {

    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

        if (policy is not null) 
        { 
            return policy;
        }

        SaasPolicy saasPolicy = new(policyName);

        // Get all instances of classes that implement ISaasRequirement and have a SaasRequirementAttribute with the same name as the policy name.
        var requirementsType = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsDefined(typeof(SaasRequirementAttribute), false))
            .Where(type => type.GetCustomAttribute<SaasRequirementAttribute>()?.PermissionEntityName == saasPolicy.GroupName)
            .Where(type => type.IsAssignableTo(typeof(ISaasRequirement)));

        // Create instances of the classes that implement ISaasRequirement and have a SaasRequirementAttribute with the same name as the policy name.
        var requirements = requirementsType.Select(type => (IAuthorizationRequirement?)Activator.CreateInstance(type, saasPolicy));

        // Create new policy builder.
        AuthorizationPolicyBuilder authorizationPolicyBuilder = new();

        // Add the default authentication requirement that the user must be authorized.
        authorizationPolicyBuilder.RequireAuthenticatedUser();

        // Add the requirements matching the policy to the policy builder.
        foreach (var requirement in requirements) 
        {
            if (requirement is not null)
            {
                authorizationPolicyBuilder.AddRequirements(requirement);
            }            
        }

        // Build the policy.
        return authorizationPolicyBuilder.Build();
    }
}
using Saas.Identity.Authorization.Model;
using Saas.Identity.Authorization.Model.Kind;

namespace Saas.Identity.Authorization.Requirement;

public abstract record SaasRequirementBase
{
    // public string PermissionClaimsIdentifier { get; } = "permissions";

    public SaasPolicy Policy { get; init; }

    protected SaasRequirementBase(SaasPolicy policy)
    {
        Policy = policy;
    }

    public abstract int PermissionValue();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected SaasRequirementBase()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }
}

using Saas.Identity.Authorization.Attribute;
using Saas.Identity.Authorization.Model;
using Saas.Identity.Authorization.Model.Data;
using Saas.Identity.Authorization.Model.Kind;

namespace Saas.Identity.Authorization.Requirement;

[SaasRequirement(TenantPermission.EntityName)]
public sealed record SaasTenantPermissionRequirement : SaasRequirementBase, ISaasRequirement
{
    public static string PermissionEntityName => TenantPermission.EntityName;

    public SaasTenantPermissionRequirement(SaasPolicy policy) : base(policy) { }

    public SaasTenantPermissionRequirement()
    {
    }

    public override int PermissionValue()
    {
        return Enum.TryParse<TenantPermissionKind>(Policy.Value, out var saasTenantPermission)
            ? (int)saasTenantPermission
            : throw new InvalidOperationException($"Unable to parse policy value '{Policy?.Value}' to enum of type {nameof(TenantPermissionKind)}");
    }
}

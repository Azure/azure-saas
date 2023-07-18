using Saas.Identity.Authorization.Attribute;
using Saas.Identity.Authorization.Model;
using Saas.Identity.Authorization.Model.Data;
using Saas.Identity.Authorization.Model.Kind;

namespace Saas.Identity.Authorization.Requirement;

[SaasRequirement(UserPermission.EntityName)]
public sealed record SaasUserPermissionRequirement : SaasRequirementBase, ISaasRequirement
{
    public static string PermissionEntityName => UserPermission.EntityName;

    public SaasUserPermissionRequirement(SaasPolicy policy) : base(policy) { }

    public SaasUserPermissionRequirement()
    {
    }

    public override int PermissionValue()
    {
        return Enum.TryParse<UserPermissionKind>(Policy.Value, out var saasUserPermission)
            ? (int)saasUserPermission
            : throw new InvalidOperationException($"Unable to parse policy value '{Policy?.Value}' to enum of type {nameof(UserPermissionKind)}");
    }
}

using Saas.Identity.Authorization.Model.Kind;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Identity.Authorization.Model.Data;

[Table("TenantPermission")]
public sealed record TenantPermission : PermissionBase, ITenantPermissionClaim
{
    [NotMapped]
    public const string EntityName = "Tenant";

    [NotMapped]
    public override string EntityIdentifier => EntityName;

    [NotMapped]
    protected override Guid EntityId => TenantId;

    [NotMapped]
    public Guid TenantId { get; init; }

    public TenantPermission(TenantPermissionKind permissionKind)
    {
        PermissionStr = permissionKind.ToString();
    }

    public TenantPermission()
    {
    }
}

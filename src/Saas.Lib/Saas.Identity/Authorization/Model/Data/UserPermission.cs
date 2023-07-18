using Saas.Identity.Authorization.Model.Kind;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Identity.Authorization.Model.Data;

[Table("UserPermission")]
public sealed record UserPermission : PermissionBase, IUserPermissionClaim
{
    [NotMapped]
    public const string EntityName = "User";

    [NotMapped]
    public override string EntityIdentifier => EntityName;

    [NotMapped]
    protected override Guid EntityId => UserId;

    [NotMapped]
    public Guid UserId { get; init; }

    public UserPermission(UserPermissionKind permissionKind)
    {
        PermissionStr = permissionKind.ToString();
    }

    public UserPermission()
    {
    }
}

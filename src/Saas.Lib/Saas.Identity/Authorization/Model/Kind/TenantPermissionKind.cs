namespace Saas.Identity.Authorization.Model.Kind;

[Flags]
public enum TenantPermissionKind
{
    None        = 0b0000,
    Create      = 0b0001,
    Read        = 0b0010,
    Update      = 0b0100,
    Delete      = 0b1000,
    Contributor = 0b0111,
    Editor      = 0b0110,
    Admin       = 0b1111
}

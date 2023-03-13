namespace Saas.Identity.Authorization.Model.Kind;

[Flags]
public enum UserPermissionKind
{
    None        = 0b00000,
    Create      = 0b00001,
    Read        = 0b00010,
    Update      = 0b00100,
    Delete      = 0b01000,
    Self        = 0b10110,
    Admin       = 0b01111,
    All         = 0b11111,
}

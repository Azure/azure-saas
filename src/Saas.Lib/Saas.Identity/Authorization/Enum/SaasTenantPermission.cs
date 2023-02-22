
namespace Saas.Identity.Authorization.Enum;

[Flags]
public enum SaasTenantPermission
{
    Create          = 0b00000001,
    Read            = 0b00000010,
    Update          = 0b00000100,
    Delete          = 0b00001000,
    TenantAdmin     = 0b11111111
}

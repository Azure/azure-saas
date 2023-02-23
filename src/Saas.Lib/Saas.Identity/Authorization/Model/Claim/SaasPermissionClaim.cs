namespace Saas.Identity.Authorization.Model.Claim;

public sealed record SaasPermissionClaim<TPermission> where TPermission : struct, Enum
{
    public static string PermissionClaimsIdentifier { get; } = "permissions";

    public bool IsValid { get; init; }

    public Guid Entity { get; init; }

    public int ToInt() => Convert.ToInt32(Permission);

    public TPermission Permission {get; init; }

    public SaasPermissionClaim(string permissionStr, string permissionSetName)
    {
        var permisionElements = permissionStr.Split('.');

        if (permisionElements[0] != permissionSetName)
        {
            IsValid = false;
            return;
        }

        if (permisionElements.Length != 3)
        {
            throw new ArgumentException($"Invalid permission string '{permissionStr}'. Should have precisely three elements, seperated by dots. This string has {permisionElements.Length} elements.");
        }

        Entity = Guid.TryParse(permisionElements[1], out Guid entityId)
            ? entityId
            : throw new ArgumentException($"Invalid permission string '{permissionStr}'. Middle element with value '{permisionElements[1]}' is not recognized as a Guid.");

        if (Enum.TryParse(permisionElements[2], out TPermission permission))
        {
            Permission = permission;
            IsValid = true;
        }
    }
}

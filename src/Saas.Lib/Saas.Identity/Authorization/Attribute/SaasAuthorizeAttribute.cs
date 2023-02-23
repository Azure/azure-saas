using Microsoft.AspNetCore.Authorization;
using Saas.Identity.Authorization.Requirement;

namespace Saas.Identity.Authorization.Attribute;

/// <summary>
/// Saas Authorize Attribute
/// </summary>
/// <typeparam name="TRequirement"></typeparam>
/// <typeparam name="TPermission"></typeparam>
public sealed class SaasAuthorizeAttribute<TRequirement, TPermission> : AuthorizeAttribute
    where TRequirement : ISaasRequirement, new()
    where TPermission : struct, Enum
{
    /// <summary>
    /// Saas Authorize Attribute
    /// </summary>
    /// <param name="permissionValue">Permission Enum.</param>
    /// <param name="routingRestrictionKeyName">Use a rounting variable value to tighten permission.</param>
    public SaasAuthorizeAttribute(
        TPermission permissionValue,
        string? routingRestrictionKeyName = default)
    {
        Policy = $"{TRequirement.PermissionEntityName}.{permissionValue}";

        if (!string.IsNullOrEmpty(routingRestrictionKeyName))
        {
            Policy += $".{routingRestrictionKeyName}";
        }
    }
}

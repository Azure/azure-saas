using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Saas.Identity.Authorization.Model.Claim;
using Saas.Identity.Authorization.Option;
using Saas.Identity.Authorization.Requirement;
using System.Collections;
using System.Security.Claims;

namespace Saas.Identity.Authorization.Handler;
public abstract class SaasPermissionAuthorizationHandlerBase<TSaasRequirement, TSaasPermissionKind>(
    IHttpContextAccessor httpContextAccessor,
    IOptions<SaasAuthorizationOptions> saasAuthorizationOptions) : AuthorizationHandler<TSaasRequirement>
    where TSaasRequirement : ISaasRequirement
    where TSaasPermissionKind : struct, Enum
{
    protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    protected readonly Guid _globalEntity = saasAuthorizationOptions?.Value.Global
            ?? throw new InvalidOperationException($"Global entity guid in '{nameof(saasAuthorizationOptions)}' cannot be null and must be defined.");

    protected virtual HashSet<int> GetGrantedPermissionValues(AuthorizationHandlerContext context, TSaasRequirement requirement)
    {
        HashSet<int> permissionsGranted;

        if (requirement.Policy.RoutingKeyName is null)
        {
            permissionsGranted = context.User.Claims
                .Where(claim => claim.Type == SaasPermissionClaim<TSaasPermissionKind>.PermissionClaimsIdentifier)
                .Select(claim => new SaasPermissionClaim<TSaasPermissionKind>(claim.Value, requirement.Policy.GroupName))
                .Where(permission => permission.IsValid)
                .Where(permission => IsValidPermission(permission, context, requirement))
                .Select(permission => permission.ToInt())
                .ToHashSet();

            return permissionsGranted;
        }

        if (_httpContextAccessor?.HttpContext?.GetRouteValue(requirement.Policy.RoutingKeyName) is not string routeValue
            || !Guid.TryParse(routeValue, out Guid requestedEntity))
        {
            requestedEntity = default;
        }

        permissionsGranted = context.User.Claims
            .Where(claim => claim.Type == SaasPermissionClaim<TSaasPermissionKind>.PermissionClaimsIdentifier)
            .Select(claim => new SaasPermissionClaim<TSaasPermissionKind>(claim.Value, requirement.Policy.GroupName))
            .Where(permission => permission.IsValid)
            .Where(permission => IsValidPermission(permission, context, requirement, requestedEntity))
            .Select(permission => permission.ToInt())
            .ToHashSet();

        return permissionsGranted;
    }

    protected virtual SaasPermissionClaim<TSaasPermissionKind> CreateSaasPermission(string value, string permissionName) => new(value, permissionName);

    protected virtual bool IsValidPermission(
        SaasPermissionClaim<TSaasPermissionKind> permission, 
        AuthorizationHandlerContext context, 
        TSaasRequirement requirement) => true;

    protected virtual bool IsValidPermission(
        SaasPermissionClaim<TSaasPermissionKind> permission,
        AuthorizationHandlerContext context,
        TSaasRequirement requirement,
        Guid routingEntity) => permission.Entity == routingEntity;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TSaasRequirement requirement)
    {
        if (requirement.Policy.Value is null)
        {
            throw new InvalidOperationException($"No permission specified for ${nameof(requirement)}.");
        }

        if (IsGlobalAdmin(context))
        {
            context.Succeed(requirement);
            return;
        }

        HashSet<int> permissionsGranted = GetGrantedPermissionValues(context, requirement);

        if (!permissionsGranted.Any())
        {
            return;
        }

        if (requirement.PermissionValue() == 0b0 // special case where the permissions needed are "None".
            || HasRequiredPermissions(requirement, permissionsGranted))
        {
            context.Succeed(requirement);
        }

        await Task.CompletedTask;
    }

    protected virtual bool IsGlobalAdmin(AuthorizationHandlerContext context)
    {
        var userId = context.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;

        if (Guid.TryParse(userId, out var userIdGuid))
        {
            userIdGuid = default;
        }

        return userIdGuid == _globalEntity;
    }

    protected virtual bool HasRequiredPermissions(TSaasRequirement requirement, HashSet<int> permissionsGranted)
    {
        BitArray requiredBits = new(requirement.PermissionValue());
        BitArray grantedBits = new(AccumulatePermissionValues(permissionsGranted));

        for (int i = 0; i < requiredBits.Count; i++)
        {
            if (requiredBits[i] == true && grantedBits[i] == false)
            {
                return false;
            }
        }

        return true;
    }

    protected virtual int AccumulatePermissionValues(HashSet<int> permissionValuesGranted)
    {
        int permissionsCombined = 0;

        foreach (var permissionValue in permissionValuesGranted)
        {
            permissionsCombined |= (int)permissionValue;
        }

        return permissionsCombined;
    }
}
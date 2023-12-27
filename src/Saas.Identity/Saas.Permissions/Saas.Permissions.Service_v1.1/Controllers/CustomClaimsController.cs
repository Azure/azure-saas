using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomClaimsController(IPermissionsService permissionsService, ILogger<CustomClaimsController> logger) : ControllerBase
{
    private readonly IPermissionsService _permissionsService = permissionsService;
    private readonly ILogger _logger = logger;

    // This is the endpoint that is called by Azure AD B2C to get alle the custom claims defined for a specific user.
    [HttpPost("permissions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PermissionsClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Permissions(ClaimsRequest request)
    {
        _logger.LogDebug("Custom claims where requested for user id: {objectId}", request.ObjectId);

        // Get all the permissions defined for the specific user with requested objectId from the database.
        var permissions = await _permissionsService.GetPermissionsAsync(request.ObjectId);

        IEnumerable<string> permissionClaims = new List<string>();

        foreach (var permission in permissions) 
        {
            // adding user permission to permissionsClaims list
            if (permission.UserPermissions?.Any() ?? false)
            {
                permissionClaims = permissionClaims
                    .Concat(permissions.SelectMany(permission => permission.UserPermissions)
                        .Select(user => user.ToClaim()));
            }

            // adding tenant permissions to permissionsClaims list
            if (permission.TenantPermissions?.Any() ?? false)
            {
                permissionClaims = permissionClaims
                    .Concat(permissions.SelectMany(permission => permission.TenantPermissions)
                        .Select(tenant => tenant.ToClaim()));
            }
        }

        PermissionsClaimResponse response = new()
        {
            Permissions = permissionClaims.ToArray()
        };

        return Ok(response);
    }

    [HttpPost("roles")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(RolesClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Roles(ClaimsRequest request)
    {
        // This request is currently retuning an empty list only.
        // The MS Graph call is expensive and we don't need it for now.
        // Also having a MS Graph call in the login flow is not ideal, as high volume of logins may hit MS Graph throttloing limits.
        // var roles = await _graphAPIService.GetAppRolesAsync(request);

        RolesClaimResponse response = new()
        {
            Roles = []
        };

        await Task.CompletedTask;

        return Ok(response);
    }
}


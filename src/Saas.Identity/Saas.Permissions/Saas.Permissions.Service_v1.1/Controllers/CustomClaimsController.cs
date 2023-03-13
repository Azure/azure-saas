using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomClaimsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;
    private readonly IGraphAPIService _graphAPIService;
    private readonly ILogger _logger;

    public CustomClaimsController(IPermissionsService permissionsService, IGraphAPIService graphAPIService, ILogger<CustomClaimsController> logger)
    {
        _permissionsService = permissionsService;
        _graphAPIService = graphAPIService;
        _logger = logger;
    }

    [HttpPost("permissions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PermissionsClaimResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Permissions(ClaimsRequest request)
    {
        _logger.LogDebug("Custom claims where requested for user id: {objectId}", request.ObjectId);

        var permissions = await _permissionsService.GetPermissionsAsync(request.ObjectId);

        IEnumerable<string> permissionClaims = new List<string>();

        foreach (var permission in permissions) 
        {
            if (permission.UserPermissions?.Any() ?? false)
            {
                permissionClaims = permissionClaims
                    .Concat(permissions
                        .SelectMany(permission => permission.UserPermissions)
                            .Select(user => user.ToClaim()));
            }

            if (permission.TenantPermissions?.Any() ?? false)
            {
                permissionClaims = permissionClaims
                    .Concat(permissions
                        .SelectMany(permission => permission.TenantPermissions)
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
        // Not using this at the moment. The MS Graph call is expensive and we don't need it for now.
        // Also having a MS Graph call in the login flow is not ideal, as high volume of logins may hit MS Graph throttloing limits.
        // var roles = await _graphAPIService.GetAppRolesAsync(request);

        RolesClaimResponse response = new()
        {
            Roles = Array.Empty<string>()
        };

        await Task.CompletedTask;

        return Ok(response);
    }
}


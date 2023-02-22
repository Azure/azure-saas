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
        var permissions = await _permissionsService.GetPermissionsAsync(request.ObjectId.ToString());

        // Append default permission with the users object ID
        string[] permissionStrings = permissions.Select(x => x.ToTenantPermissionString())                                                     
                                                     .Append($"User.{request.ObjectId}.Self")
                                                     .ToArray();
        PermissionsClaimResponse response = new()
        {
            Permissions = permissionStrings
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
        var roles = await _graphAPIService.GetAppRolesAsync(request);

        RolesClaimResponse response = new()
        {
            Roles = roles
        };

        return Ok(response);
    }
}


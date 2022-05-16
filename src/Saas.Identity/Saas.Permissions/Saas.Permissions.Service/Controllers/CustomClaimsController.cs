using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]

[ApiController]
// Specify that this controller should use Certificate Based Auth. Certificate auth is required for fetching custom claims from B2C. 
[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
public class CustomClaimsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;
    private readonly IGraphAPIService _graphAPIService;
    private readonly ILogger _logger;

    public CustomClaimsController(IPermissionsService permissionsService, IGraphAPIService graphAPIService, ILogger logger)
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
        _logger.LogDebug("Custom claims where requested for email: {EmailAddress}", request.EmailAddress);
        var permissions = await _permissionsService.GetPermissionsAsync(request.EmailAddress);

        string[] permissionStrings = permissions.Select(x => x.ToTenantPermissionString())
                                                     // Append default permission with the users object ID
                                                     .Append($"{request.ObjectId}.Self")
                                                     .ToArray();
        PermissionsClaimResponse response = new PermissionsClaimResponse()
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

        RolesClaimResponse response = new RolesClaimResponse()
        {
            Roles = roles
        };

        return Ok(response);
    }
}


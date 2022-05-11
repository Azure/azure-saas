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
    private readonly ILogger _logger;

    public CustomClaimsController(IPermissionsService permissionsService, ILogger logger)
    {
        _permissionsService = permissionsService;
        _logger = logger;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ADB2CReponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomClaims(ADB2CRequest aDB2CRequest)
    {
        _logger.LogDebug("Custom claims where requested for email: {EmailAddress}", aDB2CRequest.EmailAddress);
        var permissions = await _permissionsService.GetPermissionsAsync(aDB2CRequest.EmailAddress);

        string[] permissionStrings = permissions.Select(x => x.ToTenantPermissionString())
                                                     // Append default permission with the users object ID
                                                     .Append($"{aDB2CRequest.ObjectId}.Self")
                                                     .ToArray();
        ADB2CReponse response = new ADB2CReponse()
        {
            Permissions = permissionStrings
        };

        return Ok(response);
    }
}


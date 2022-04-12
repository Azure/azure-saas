using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Saas.Permissions.Api.Interfaces;
using Saas.Permissions.Api.Models;

namespace Saas.Permissions.Api.Controllers;

[Route("api/[controller]")]

[ApiController]
// Specify that this controller should use Certificate Based Auth. Certificate auth is required for fetching custom claims from B2C. 
[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
public class CustomClaimsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;

    public CustomClaimsController(IPermissionsService permissionsService)
    {
        _permissionsService = permissionsService;
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ADB2CReponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCustomClaims(ADB2CRequest aDB2CRequest)
    {
        var permissions = await _permissionsService.GetPermissionsAsync(aDB2CRequest.EmailAddress);

        ADB2CReponse response = new ADB2CReponse()
        {
            Permissions = permissions.Select(x => x.ToTenantPermissionString()).ToArray()
        };

        return Ok(response);
    }
}


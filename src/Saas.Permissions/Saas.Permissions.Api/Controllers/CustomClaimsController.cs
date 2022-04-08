using Microsoft.AspNetCore.Authorization;
using Saas.Permissions.Api.Interfaces;
using Saas.Permissions.Api.Models;

namespace Saas.Permissions.Api.Controllers;

[Route("api/[controller]")]

[ApiController]
[Authorize]
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


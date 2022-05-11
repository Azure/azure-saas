using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
// Specify that this controller should use Certificate Based Auth. Certificate auth is required for fetching custom claims from B2C. 
[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults .AuthenticationScheme)]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;
    private readonly ILogger _logger;

    public PermissionsController(IPermissionsService permissionsService, ILogger logger)
    {
        _permissionsService = permissionsService;
        _logger = logger;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   // [RequiredScope(new[] { "permissions.read", "permissions.write" })]
    [Route("GetTenantUsers")]
    public async Task<ICollection<string>> GetTenantUsers(string tenantId)
    {
        return await _permissionsService.GetTenantUsersAsync(tenantId);
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[RequiredScope(new[] { "permissions.read", "permissions.write" })]
    [Route("GetUserPermissionsForTenant")]
    public async Task<ICollection<string>> GetUserPermissionsForTenant(string tenantId, string userId)
    {
        return await _permissionsService.GetUserPermissionsForTenantAsync(tenantId, userId);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[RequiredScope("permissions.write")]
    [Route("AddUserPermissionsToTenant")]
    public async Task<IActionResult> AddUserPermissionsToTenant(string tenantId, string userId, string[] permissions)
    {
        try
        {
            await _permissionsService.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
            return Ok();
        }
        catch (ItemAlreadyExistsException ex)
        {
            _logger.LogError("Permissions where not able to be added to {userId} on {tenantId}", userId, tenantId);
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[RequiredScope("permissions.write")]
    [Route("RemoveUserPermissionsFromTenant")]
    public async Task<IActionResult> RemoveUserPermissionsFromTenant(string tenantId, string userId, string[] permissions)
    {
        try
        {
            await _permissionsService.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
            return Ok();
        }
        catch (ItemNotFoundExcepton ex)
        {
            _logger.LogError("Permissions where not removed from {userId} on {tenantId}", userId, tenantId); 
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //[RequiredScope(new[] { "permissions.read", "permissions.write" })]
    [Route("GetTenantsForUser")]
    public async Task<ICollection<string>> GetTenantsForUser(string userId, string? filter)
    {
        // filter not currently implemented.

        return await _permissionsService.GetTenantsForUserAsync(userId, filter);
    }
}


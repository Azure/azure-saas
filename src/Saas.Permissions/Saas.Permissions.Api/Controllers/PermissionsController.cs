using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Saas.Permissions.Api.Exceptions;
using Saas.Permissions.Api.Interfaces;
using System.Net;

namespace Saas.Permissions.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;

    public PermissionsController(IPermissionsService permissionsService)
    {
        _permissionsService = permissionsService;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [RequiredScope(new[] { "permissions.read", "permissions.write" })]
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
    [RequiredScope(new[] { "permissions.read", "permissions.write" })]
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
    [RequiredScope("permissions.write")]
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
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [RequiredScope("permissions.write")]
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
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [RequiredScope(new[] { "permissions.read", "permissions.write" })]
    [Route("GetTenantsForUser")]
    public async Task<ICollection<string>> GetTenantsForUser(string userId, string? filter)
    {
        // filter not currently implemented.

        return await _permissionsService.GetTenantsForUserAsync(userId, filter);
    }
}


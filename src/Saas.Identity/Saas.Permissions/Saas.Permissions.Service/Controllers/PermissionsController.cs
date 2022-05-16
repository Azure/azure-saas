using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults .AuthenticationScheme)]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionsService _permissionsService;
    private readonly IGraphAPIService _graphAPIService;

    public PermissionsController(IPermissionsService permissionsService, IGraphAPIService graphAPIService)
    {
        _permissionsService = permissionsService;
        _graphAPIService = graphAPIService;
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("GetTenantUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetTenantUsers(string tenantId)
    {
        // Get user IDs from database
        ICollection<string> userIds = await _permissionsService.GetTenantUsersAsync(tenantId);
        
        //
        // Next, we fetch the user objects with more data from the Microsoft Graph API
        //
        // We chose to not force the UserIDs to be GUIDs incase you would like to use something else for IDs.
        // Since we're using AAD B2C as our default Identity provider and need to get data from the Graph API, we must first make sure our IDs are guids before sending them to graph.
        // If you are not using our identity framework, you will need to replace the following try/catch block with an implementation to fetch your user information from your user store.
        try
        {
            var enrichedUsers = await _graphAPIService.GetUsersByIds(userIds.Select(stringId => Guid.Parse(stringId)).ToList());
            return Ok(enrichedUsers);
        } 
        catch (FormatException ex)
        {
            return BadRequest($"Tenant ID {tenantId} has a user assinged that has an invalid ID. Error: {ex.Message}");
            throw;
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [Route("GetTenantsForUser")]
    public async Task<ICollection<string>> GetTenantsForUser(string userId, string? filter)
    {
        // filter not currently implemented.

        return await _permissionsService.GetTenantsForUserAsync(userId, filter);
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("GetUsersByIds")]
    public async Task<IActionResult> GetUsersByIds(string[] userIds)
    {
        try
        {
            var users = await _graphAPIService.GetUsersByIds(userIds.Select(stringId => Guid.Parse(stringId)).ToList());
            return Ok(users);
        }
        catch (FormatException ex)
        {
            return BadRequest($"UserIds provided to the Microsoft Graph API must be GUIDs. Error: {ex.Message}");
            throw;
        }
    }


}


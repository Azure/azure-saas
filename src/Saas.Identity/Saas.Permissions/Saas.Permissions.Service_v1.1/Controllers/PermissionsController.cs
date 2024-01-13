using Saas.Permissions.Service.Exceptions;
using Saas.Permissions.Service.Interfaces;
using Saas.Permissions.Service.Models;

namespace Saas.Permissions.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsController(
    IPermissionsService permissionsService, 
    IGraphAPIService graphAPIService, ILogger<PermissionsController> logger) : ControllerBase
{
    private readonly ILogger _logger = logger;

    private readonly IPermissionsService _permissionsService = permissionsService;
    private readonly IGraphAPIService _graphAPIService = graphAPIService;

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("GetTenantUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetTenantUsers(Guid tenantId)
    {
        // Get user IDs from database
        ICollection<Guid> userIds;

        try
        {
            userIds = await _permissionsService.GetTenantUsersAsync(tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to get Tenant Users: {ex}", ex);
            throw;
        }

        if (userIds is null || userIds.Count == 0)
        {
            return Ok(new List<User>());
        }
                    
        //
        // Next, we fetch the user objects with more data from the Microsoft Graph API
        //
        // We chose to not force the UserIDs to be GUIDs incase you would like to use something else for IDs.
        // Since we're using AAD B2C as our default Identity provider and need to get data from the Graph API, we must first make sure our IDs are guids before sending them to graph.
        // If you are not using our identity framework, you will need to replace the following try/catch block with an implementation to fetch your user information from your user store.
        try
        {
            var enrichedUsers = await _graphAPIService
                .GetUsersByIds(userIds);

            return Ok(enrichedUsers);
        } 
        catch (FormatException ex)
        {
            return BadRequest($"Tenant ID {tenantId} has a user assinged that has an invalid ID. Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception: {ex}", ex);
            throw;
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("GetTenantUser")]
    public async Task<ActionResult<User>> GetTenantUser(Guid tenantId, Guid userId)
    {
        // Get user ID from database
        Guid? permissionUserId = null;

        try
        {
            permissionUserId = await _permissionsService.GetTenantUserAsync(tenantId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to get Tenant User: {ex}", ex);
            throw;
        }

        if (permissionUserId is null || permissionUserId.Value == Guid.Empty)
        {
            return NotFound();
        }

        //
        // Next, we fetch the user object with more data from the Microsoft Graph API
        //
        // We chose to not force the UserID to be GUID incase you would like to use something else for ID.
        // Since we're using AAD B2C as our default Identity provider and need to get data from the Graph API, we must first make sure our ID is a guid before sending them to graph.
        // If you are not using our identity framework, you will need to replace the following try/catch block with an implementation to fetch your user information from your user store.
        try
        {
            var enrichedUser = await _graphAPIService
                .GetUserById(permissionUserId.Value);

            return Ok(enrichedUser);
        } 
        catch (FormatException ex)
        {
            return BadRequest($"Tenant ID {tenantId} has a user assinged that has an invalid ID. Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("Unhandled exception: {ex}", ex);
            throw;
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("GetUserPermissionsForTenant")]
    public async Task<ICollection<string>> GetUserPermissionsForTenant(Guid tenantId, Guid userId)
    {
        return await _permissionsService.GetUserPermissionClaimsForTenantAsync(tenantId, userId);
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("AddNewTenant")]
    public async Task<IActionResult> AddNewTenant(Guid tenantId, Guid userId)
    {
        try
        {
            await _permissionsService.AddNewTenantAsync(tenantId, userId);
            return Ok();
        }
        catch (ItemAlreadyExistsException ex)
        {
            _logger.LogError("Permissions were not able to be added to {userId} on {tenantId}", userId, tenantId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("AddUserPermissionsToTenant")]
    public async Task<IActionResult> AddUserPermissionsToTenant(Guid tenantId, Guid userId, string[] permissions)
    {
        try
        {
            await _permissionsService.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
            return Ok();
        }
        catch (ItemAlreadyExistsException ex)
        {
            _logger.LogError("Permissions were not able to be added to {userId} on {tenantId}", userId, tenantId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("AddUserPermissionsToUser")]
    public async Task<IActionResult> AddUserPermissionsToUser(Guid tenantId, Guid userId, string[] permissions)
    {
        try
        {
            await _permissionsService.AddUserPermissionsToUserAsync(tenantId, userId, permissions);
            return Ok();
        }
        catch (ItemAlreadyExistsException ex)
        {
            _logger.LogError("Permissions were not able to be added to {userId}.", userId);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Route("AddUserPermissionsToTenantByEmail")]
    public async Task<IActionResult> AddUserPermissionsToTenantByEmail(Guid tenantId, string userEmail, string[] permissions)
    {
        try
        {
            await _permissionsService.AddUserPermissionsToTenantByEmailAsync(tenantId, userEmail, permissions);
            return Ok();
        }
        catch (ItemAlreadyExistsException ex)
        {
            _logger.LogError("Permissions where not able to be added to {userEmail} on {tenantId}. Permission already exists.", userEmail, tenantId);
            return BadRequest(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogError("User: {userEmail} was not found in the identity provider or more than one user exists with that email. permissions could not be added on {tenantId}", userEmail, tenantId);
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
    public async Task<IActionResult> RemoveUserPermissionsFromTenant(Guid tenantId, Guid userId, string[] permissions)
    {
        try
        {
            await _permissionsService.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
            return Ok();
        }
        catch (ItemNotFoundException ex)
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
    [Route("GetTenantsForUser")]
    public async Task<ICollection<Guid>> GetTenantsForUser(Guid userId)
    {
        return await _permissionsService.GetTenantsForUserAsync(userId);
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
        if (userIds.Length <= 0) 
        {
            return BadRequest("You must provide at least one userID");
        }

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


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

    public PermissionsController(IPermissionsService permissionsService)
    {
        _permissionsService = permissionsService;
    }


    /// <summary>
    /// Get all users for a tenant 
    /// </summary>
    /// <returns>List of all users for a tenant</returns>
    /// <remarks>
    /// <para><b>Requires:</b> a valid tenant ID </para>
    /// <para>This call will return a list of all users in the system</para>
    /// </remarks>
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

    /// <summary>
    /// Get the permissions for a user for a tenant
    /// </summary>
    /// <returns>A list of permissions for a given user and tenant</returns>
    /// <remarks>
    /// <para><b>Requires:</b> a valid tenant ID </para>
    /// <para><b>Requires:</b> a valid User ID </para>
    /// <para>This call will return a list of all permissions a user has in a tenant</para>
    /// </remarks>
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

    /// <summary>
    /// Adds a permissions to a user on a given tenant
    /// </summary>
    /// <returns>A status code of 200 if successful</returns>
    /// <remarks>
    /// <para><b>Requires:</b> a valid tenant ID </para>
    /// <para><b>Requires:</b> a valid User ID </para>
    /// <para><b>Requires</b> an array of valid permissions</para>
    /// <para>This call will return a 200 code if successful</para>
    /// </remarks>
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


    /// <summary>
    /// Removes the permissions for a user for a tenant
    /// </summary>
    /// <returns>Returns a 200 code if successful</returns>
    /// <remarks>
    /// <para><b>Requires:</b> a valid tenant ID </para>
    /// <para><b>Requires:</b> a valid User ID </para>
    /// <para><b>Requires</b> an array of valid permissions</para>
    /// <para>Returns a 200 code if successful</para>
    /// </remarks>
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
            return BadRequest(ex.Message);
        }
    }


    /// <summary>
    /// Get a list of tenants that a user has access to
    /// </summary>
    /// <returns>A list of tenant IDs  that a user has access to</returns>
    /// <remarks>
    /// <para><b>Requires:</b> a valid User ID </para>
    /// <para>Filters will be implemented in the future</para>
    /// <para>This call will return a list tenant IDs that the user has access to</para>
    /// </remarks>
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


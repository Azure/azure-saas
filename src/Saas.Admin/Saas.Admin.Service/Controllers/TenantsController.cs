#nullable disable

namespace Saas.Admin.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger _logger;

    public TenantsController(ITenantService tenantService, IPermissionService permissionService, ILogger<TenantsController> logger)
    {
        _logger = logger;
        _tenantService = tenantService;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Get all tenants in the system
    /// </summary>
    /// <returns>List of all tenants</returns>
    /// <remarks>
    /// <para><b>Requires:</b> admin.tenant.read</para>
    /// <para>This call will return all the tenants in the system.</para>
    /// </remarks>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TenantDTO>>> GetAllTenants()
    {
        IEnumerable<Tenant> allTenants = await _tenantService.GetAllTenantsAsync();
        return allTenants.Select(s => new TenantDTO(s)).ToList();
    }

    /// <summary>
    /// Get a tenant by tenant ID
    /// </summary>
    /// <param name="tenantId">Guid representing the tenant</param>
    /// <returns>Information about the tenant</returns>
    /// <remarks>
    /// <para><b>Requires:</b> admin.tenant.read  or  {tenantId}.tenant.read</para>
    /// <para>Will return details of a single tenant, if user has access.</para>
    /// </remarks>
    [HttpGet("{tenantId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TenantDTO>> GetTenant(Guid tenantId)
    {
        try
        {
            var tenant = await _tenantService.GetTenantAsync(tenantId);
            return new TenantDTO(tenant);
        }
        catch (ItemNotFoundExcepton)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Add a new tenant
    /// </summary>
    /// <param name="tenantRequest"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para><b>Requires:</b> Authenticated user</para>
    /// <para>This call needs a user to make admin of this tenant.  TBD explicitly pass in the user ID or 
    /// make the current user the admin (would prevent a third party creating tenants on behalf of user)</para>
    /// </remarks>
    [HttpPost()]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TenantDTO>> PostTenant(NewTenantRequest tenantRequest)
    {

        Tenant tenant = tenantRequest.ToTenant();
        tenant = await _tenantService.AddTenantAsync(tenant);

        return CreatedAtAction("GetTenant", new { id = tenant.Id }, tenant);
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="tenantDTO"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para><b>Requires:</b> admin.tenant.write  or  {tenantId}.tenant.write</para>
    /// </remarks>
    [HttpPut("{tenantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PutTenant(Guid tenantId, TenantDTO tenantDTO)
    {
        if (tenantId != tenantDTO.Id)
        {
            return BadRequest();
        }

        try
        {
            await _tenantService.UpdateTenantAsync(tenantDTO.ToTenant());
        }
        catch (ItemNotFoundExcepton)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [HttpDelete("{tenantId}")]
    public async Task<IActionResult> DeleteTenant(Guid tenantId)
    {
        try
        {
            await _tenantService.DeleteTenantAsync(tenantId);
        }
        catch (ItemNotFoundExcepton)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Get all users associated with a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>Right now only returns user IDs, should consider returning a user object with 
    /// user info + permissions for the tenant</para>
    /// </remarks>
    [HttpGet("{tenantId}/users")]
    public async Task<ActionResult<IEnumerable<string>>> GetTenantUsers(Guid tenantId)
    {
        IEnumerable<string> users = await _permissionService.GetTenantUsersAsync(tenantId);
        return users.ToList();
    }

    /// <summary>
    /// Get all permissions a user has in a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <remarks>This might be better combined with GetTenantUsers, all usescases seem like they would need both</remarks>
    [HttpGet("{tenantId}/Users/{userId}/permissions")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(Guid tenantId, string userId)
    {
        var permissions = await _permissionService.GetUserPermissionsForTenantAsync(tenantId, userId);
        return permissions.ToList();
    }

    /// <summary>
    /// Add a set of permissions for a user on a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
    [HttpPost("{tenantId}/Users/{userId}/permissions")]
    public async Task<IActionResult> PostUserPermissions(Guid tenantId, string userId, [FromBody] string[] permissions)
    {
        await _permissionService.AddUserPermissionsToTenantAsyc(tenantId, userId, permissions);
        return NoContent();
    }

    /// <summary>
    /// Delete a set of permissions for a user on a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
    [HttpDelete("{tenantId}/Users/{userId}/permissions")]
    public async Task<IActionResult> DeleteUserPermissions(Guid tenantId, string userId, [FromBody] string[] permissions)
    {
        await _permissionService.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
        return NoContent();
    }

    /// <summary>
    /// Get all tenant IDs that a user has access to
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="filter">Optionally filter by access type</param>
    /// <returns></returns>
    [HttpGet("user/{userId}/tenants")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    //sysadmin or current user
    public async Task<ActionResult<IEnumerable<Guid>>> UserTenants(string userId, string filter = null)
    {
        this._logger.LogDebug("Geting all tenants for user {userId}", userId);

        IEnumerable<Guid> tenants = await _permissionService.GetTenantsForUserAsync(userId, filter);
        return tenants.ToList();
    }
}

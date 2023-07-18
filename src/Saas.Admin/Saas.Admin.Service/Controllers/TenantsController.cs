using Saas.Identity.Authorization.Attribute;
using Saas.Identity.Authorization.Model.Claim;
using Saas.Identity.Authorization.Model.Data;
using Saas.Identity.Authorization.Model.Kind;
using Saas.Identity.Authorization.Requirement;
using Saas.Permissions.Client;
using System.Net.Mime;

namespace Saas.Admin.Service.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IPermissionsServiceClient _permissionsServiceClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    public TenantsController(
        ITenantService tenantService, 
        IPermissionsServiceClient permissionService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TenantsController> logger)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _tenantService = tenantService;
        _permissionsServiceClient = permissionService;
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
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Read)]
    public async Task<ActionResult<IEnumerable<TenantDTO>>> GetAllTenants()
    {
        try
        {
            _logger.LogDebug("{UserName} is requesting all tenants.", User?.Identity?.Name);

            List<TenantDTO> allTenants = (await _tenantService.GetAllTenantsAsync()).ToList();

            _logger.LogDebug("Returning {ReturnCount} tenants", allTenants.Count);
            return Ok(allTenants);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem retrieving all tenants");
            throw;
        }
    }

    /// <summary>
    /// Get a tenant by tenant ID
    /// </summary>
    /// <param name="tenantId">Guid representing the tenant</param>
    /// <returns>Information about the tenant</returns>
    /// <remarks>
    /// <para><b>Requires:</b> admin.tenant.read  or  {tenantID}.tenant.read</para>
    /// <para>Will return details of a single tenant, if user has access.</para>
    /// </remarks>
    [HttpGet("{tenantId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Read, routingRestrictionKeyName: "tenantId")]
    public async Task<ActionResult<TenantDTO>> GetTenant(Guid tenantId)
    {
        _logger.LogDebug("{User} requested tenant with ID {TenantID}", User?.Identity?.Name, tenantId);
        try
        {
            TenantDTO tenant = await _tenantService.GetTenantAsync(tenantId);
            _logger.LogDebug("Found {TenantName} with {TenantID}", tenant.Name, tenantId);

            return Ok(tenant);
        }
        catch (ItemNotFoundExcepton)
        {
            _logger.LogDebug("Was not able to find tenant with ID {TeantntID}", tenantId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem retrieving tenant with ID {TeantntID}", tenantId);
            throw;
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
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(TenantDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    [Authorize] 
    public async Task<ActionResult<TenantDTO>> PostTenant(NewTenantRequest tenantRequest)
    {
        try
        {
            _logger.LogInformation("Creating a new tenant: {NewTenantName} for {OwnerID}, requested by {User}", tenantRequest.Name, tenantRequest.CreatorEmail, User?.Identity?.Name);
            
            if (! Guid.TryParse(User?.GetNameIdentifierId(), out var userId)) 
            {
                throw new InvalidOperationException("The the User Name Identifier must be a Guid.");
            }
            
            TenantDTO tenant = await _tenantService.AddTenantAsync(tenantRequest, userId);

            _logger.LogInformation("Created a new tenant {NewTenantName} with URL {NewTenantRoute}, and ID {NewTenantID}", tenant.Name, tenant.Route, tenant.Id);
            
            return CreatedAtAction(nameof(GetTenant), new { tenantId = tenant.Id }, tenant);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest((ex.InnerException ?? ex).Message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem creating tenant with ID {TenantName}", tenantRequest.Name);
            throw;
        }
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="tenantDTO"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para><b>Requires:</b> admin.tenant.write  or  {tenantID}.tenant.write</para>
    /// </remarks>
    [HttpPut("{tenantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Update, "tenantId")]
    public async Task<IActionResult> PutTenant(Guid tenantId, TenantDTO tenantDTO)
    {
        _logger.LogDebug("Updating tenant {TenantID} by {User}", tenantId, User?.Identity?.Name);
        if (tenantId != tenantDTO.Id)
        {
            _logger.LogInformation("Requested Id {TenantID} did not match request data {DTOTenantID}", tenantId, tenantDTO.Id);
            return BadRequest();
        }
        try
        {
            await _tenantService.UpdateTenantAsync(tenantDTO);
            _logger.LogInformation("Updated tenant {TenantName} with id {TenantID}", tenantDTO.Name, tenantDTO.Id);
        }
        catch (ItemNotFoundExcepton ex)
        {
            _logger.LogWarning(ex, "Unable to find tenant {TenantID}", tenantId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem updating tenant {TenantID}", tenantId);
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    [HttpDelete("{tenantId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Delete, "tenantId")]
    public async Task<IActionResult> DeleteTenant(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Deleting tenant {TenantID} by {User}", tenantId, User?.Identity?.Name);
            await _tenantService.DeleteTenantAsync(tenantId);
        }
        catch (ItemNotFoundExcepton ex)
        {
            _logger.LogWarning(ex, "Unable to find tenant {TenantID}", tenantId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to delete tenant {TeanantID}", tenantId);
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Get public tenant info by route
    /// </summary>
    /// <param name="route">String route of tenant</param>
    /// <returns>Information about the tenant</returns>
    /// <remarks>
    /// <para><b>Requires:</b>Authorize</para>
    /// <para>Will return public details of a single tenant</para>
    /// </remarks>
    [HttpGet("tenantinfo/{route}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Read)]
    public async Task<ActionResult<TenantInfoDTO>> GetTenantInfoByRoute(string route)
    {
        _logger.LogDebug("{User} requested tenant for route {Route}", User?.Identity?.Name, route);

        try
        {
            var tenantPermissions = _httpContextAccessor?.HttpContext?.User.Claims
                .Where(c => c.Type == SaasPermissionClaim<TenantPermissionKind>.PermissionClaimsIdentifier)
                .Select(claim => new SaasPermissionClaim<TenantPermissionKind>(claim.Value, TenantPermission.EntityName))
                .Where(permission => permission.IsValid);

            if (tenantPermissions is null)
            {
                _logger.LogDebug("No tenant permissions for looking up {Route}", route);
                return NotFound();
            }

            var tenant = await _tenantService.GetTenantInfoByRouteAsync(route);

            if (tenant is null)
            {
                _logger.LogDebug("Was not able to find tenant with route {Route}", route);
                return NotFound();
            }

            if (tenantPermissions.Any(permission => permission.Entity == tenant.Id))
            {
                _logger.LogDebug("Found {TenantName} with route {Route}", tenant.Name, route);

                return Ok(tenant);
            }
            else
            {
                _logger.LogDebug("Found {TenantName} with route {Route}, but requesting user does not have access to it.", tenant.Name, route);
                return NotFound();
            }
        }
        catch (ItemNotFoundExcepton)
        {
            _logger.LogDebug("Was not able to find tenant with route {Route}", route);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem retrieving tenant with route {Route}", route);
            throw;
        }
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
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [Route("{tenantId}/users")]
    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(permissionValue: TenantPermissionKind.Read, "tenantId")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetTenantUsers(Guid tenantId)
    {
        try
        {
            _logger.LogDebug("Retrieving users for tenant {TenantID} by {User}", tenantId, User?.Identity?.Name);

            ICollection<User>? users = await _permissionsServiceClient.GetTenantUsersAsync(tenantId);

            List<UserDTO> returnValue = users.Select(u => new UserDTO(u.UserId, u.DisplayName)).ToList();

            _logger.LogDebug("Returning {UserCount} users for tenant {TenantID} to {User}", returnValue.Count, tenantId, User?.Identity?.Name);
            return Ok(returnValue);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Problem retrieving users for {TenantID}", tenantId);
            throw;
        }
    }

    /// <summary>
    /// Get all permissions a user has in a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <remarks>This might be better combined with GetTenantUsers, all usescases seem like they would need both</remarks>
    [HttpGet("{tenantId}/Users/{userId}/permissions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Read, "tenantId")]
    [SaasAuthorize<SaasUserPermissionRequirement, UserPermissionKind>(UserPermissionKind.Read, "userId")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserPermissions(Guid tenantId, Guid userId)
    {
        IEnumerable<string> permissions = await _permissionsServiceClient.GetUserPermissionsForTenantAsync(tenantId, userId);
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Admin, "tenantId")]
    [SaasAuthorize<SaasUserPermissionRequirement, UserPermissionKind>(UserPermissionKind.Create, "userId")]
    public async Task<IActionResult> PostUserPermissions(Guid tenantId, Guid userId, [FromBody] string[] permissions)
    {
        await _permissionsServiceClient.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
        return NoContent();
    }

    /// <summary>
    /// Add a set of permissions for a user on a tenant
    /// </summary>
    /// <param name="tenantId"></param>
    /// <param name="userEmail"></param>
    /// <returns></returns>
    [HttpPost("{tenantId}/invite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Admin, "tenantId")]
    public async Task<IActionResult> InviteUserToTenant(Guid tenantId, string userEmail)
    {
        await _permissionsServiceClient.AddUserPermissionsToTenantByEmailAsync(
            tenantId, 
            userEmail, 
            new string[] { TenantPermissionKind.Admin.ToString() });
        
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasTenantPermissionRequirement, TenantPermissionKind>(TenantPermissionKind.Admin, "tenantId")]
    [SaasAuthorize<SaasUserPermissionRequirement, UserPermissionKind>(UserPermissionKind.Delete, "userId")]
    public async Task<IActionResult> DeleteUserPermissions(Guid tenantId, Guid userId, [FromBody] string[] permissions)
    {
        await _permissionsServiceClient.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
        return NoContent();
    }

    /// <summary>
    /// Get all tenant IDs that a user has access to
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("user/{userId}/tenants")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [SaasAuthorize<SaasUserPermissionRequirement, UserPermissionKind>(UserPermissionKind.Self, "userId")]
    public async Task<ActionResult<IEnumerable<TenantDTO>>> UserTenants(Guid userId)
    {
        _logger.LogDebug("Getting all tenants for user {userID}", userId);

        IEnumerable<Guid> tenantIds = await _permissionsServiceClient.GetTenantsForUserAsync(userId);
        IEnumerable<TenantDTO>? tenants = await _tenantService.GetTenantsByIdAsync(tenantIds);
        return tenants.ToList();
    }

    [HttpGet("IsValidPath/{path}")]
    [ProducesResponseType(StatusCodes.Status200OK)]

    [Authorize]
    public async Task<ActionResult<bool>> IsValidPath(string path)
    {
        _logger.LogDebug("Validating Path {path}", path);

        bool pathExists = await _tenantService.CheckPathExists(path);
        return !pathExists;
    }
}

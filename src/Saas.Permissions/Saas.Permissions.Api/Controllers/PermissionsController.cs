
using Saas.Permissions.Api.Data;
using Saas.Permissions.Api.Interfaces;

namespace Saas.Permissions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        [HttpGet]
        [Route("GetTenantUsers")]
        public async Task<ICollection<Guid>> GetTenantUsers(Guid tenantId)
        {
            return await _permissionsService.GetTenantUsersAsync(tenantId);
        }

        [HttpGet]
        [Route("GetUserPermissionsForTenant")]
        public async Task<ICollection<string>> GetUserPermissionsForTenant(Guid tenantId, string userId)
        {
            return await _permissionsService.GetUserPermissionsForTenantAsync(tenantId, userId);
        }

        [HttpPost]
        [Route("AddUserPermissionsToTenant")]
        public async Task<IActionResult> AddUserPermissionsToTenant(Guid tenantId, string userId, string[] permissions)
        {
            await _permissionsService.AddUserPermissionsToTenantAsync(tenantId, userId, permissions);
            return Ok();
        }

        [HttpDelete]
        [Route("RemoveUserPermissionsFromTenant")]
        public async Task<IActionResult> RemoveUserPermissionsFromTenant(Guid tenantId, string userId, string[] permissions)
        {
            await _permissionsService.RemoveUserPermissionsFromTenantAsync(tenantId, userId, permissions);
            return Ok();
        }

        [HttpGet]
        [Route("GetTenantsForUser")]
        public async Task<ICollection<Guid>> GetTenantsForUser(string userId, string? filter)
        {
            // filter not currently implemented.

            return await _permissionsService.GetTenantsForUserAsync(userId, filter); 
        }
    }
}

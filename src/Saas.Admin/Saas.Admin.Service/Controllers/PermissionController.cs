using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Saas.Admin.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> TestRoute()
        {
            return await _permissionService.GetTenantUsersAsync(new Guid());
        }

    }
}

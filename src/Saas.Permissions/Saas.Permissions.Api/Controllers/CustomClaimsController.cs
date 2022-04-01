using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas.Permissions.Api.Data;
using Saas.Permissions.Api.Interfaces;
using Saas.Permissions.Api.Models;
using System.Text.Json;

namespace Saas.Permissions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomClaimsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public CustomClaimsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService; 
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomClaims(ADB2CRequest aDB2CRequest)
        {
            var permissions = await _permissionsService.GetPermissionsAsync(aDB2CRequest.ObjectId);

            ADB2CReponse response = new ADB2CReponse()
            {
                Permissions = permissions.Select(x => $"{x.TenantId}.{x.Role}").ToArray()
            };

            return Ok(response);
        }
    }
}

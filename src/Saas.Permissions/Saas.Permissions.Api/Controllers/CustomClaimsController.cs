using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas.Permissions.Api.Models;
using System.Text.Json;

namespace Saas.Permissions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomClaimsController : ControllerBase
    {
        [HttpPost]
        public IActionResult GetCustomClaims(ADB2CRequest aDB2CRequest)
        {
            var responseContent = new ResponseContent()
            {
                extension_CustomClaim = "9183cdfa-c406-42cb-9e86-dee61ca2a324.Admin"
            };

            return Ok(responseContent);
        }
    }
}

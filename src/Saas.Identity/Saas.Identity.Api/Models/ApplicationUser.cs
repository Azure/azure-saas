using Microsoft.AspNetCore.Identity;

namespace Saas.Identity.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string TenantId { get; set; }
        public string Created { get; set; }
    }
}

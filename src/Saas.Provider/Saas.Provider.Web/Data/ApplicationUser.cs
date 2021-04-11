using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string TenantId { get; set; }
    }
}

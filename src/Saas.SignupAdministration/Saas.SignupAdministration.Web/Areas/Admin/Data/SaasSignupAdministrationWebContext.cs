using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas.SignupAdministration.Web.Models;

namespace Saas.SignupAdministration.Web.Data
{
    public class SaasSignupAdministrationWebContext : DbContext
    {
        public SaasSignupAdministrationWebContext (DbContextOptions<SaasSignupAdministrationWebContext> options)
            : base(options)
        {
        }

        public DbSet<Saas.SignupAdministration.Web.Models.Tenant> Tenant { get; set; }
    }
}

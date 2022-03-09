#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Saas.Admin.Service.Data
{
    public class SubscriptionsContext : DbContext
    {
        public SubscriptionsContext (DbContextOptions<SubscriptionsContext> options)
            : base(options)
        {
        }

        public DbSet<Saas.Admin.Service.Data.Subscription> Subscription { get; set; }
    }
}

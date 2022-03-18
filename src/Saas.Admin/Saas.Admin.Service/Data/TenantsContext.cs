#nullable disable

namespace Saas.Admin.Service.Data;

public class TenantsContext : DbContext
{
    public TenantsContext(DbContextOptions<TenantsContext> options)
        : base(options)
    {

    }

    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new TenantEntityTypeConfiguration().Configure(modelBuilder.Entity<Tenant>());
    }
}

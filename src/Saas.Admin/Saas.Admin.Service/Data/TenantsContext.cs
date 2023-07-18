namespace Saas.Admin.Service.Data;

public class TenantsContext : DbContext
{
    public TenantsContext(DbContextOptions<TenantsContext> options)
        : base(options)
    {

    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
        new TenantEntityTypeConfiguration().Configure(modelBuilder.Entity<Tenant>());
    }
}

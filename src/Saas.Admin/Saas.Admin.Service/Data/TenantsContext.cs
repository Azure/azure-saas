using Saas.Admin.Service.Data.Models.OnBoarding;

namespace Saas.Admin.Service.Data;

public class TenantsContext : DbContext
{
    public TenantsContext(DbContextOptions<TenantsContext> options)
        : base(options)
    {

    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<UserInfo> UserInfo => Set<UserInfo>();

    public DbSet<UserTenant> UserTenants => Set<UserTenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        TenantEntityTypeConfiguration configuration = new TenantEntityTypeConfiguration();
        configuration.Configure(modelBuilder.Entity<Tenant>());
        configuration.Configure(modelBuilder.Entity<UserTenant>());


        modelBuilder.Entity<UserTenant>()
            .HasOne(ut => ut.UserInfo)
            .WithMany(ui => ui.UserTenants)
            .HasForeignKey(ut => ut.UserId);

        modelBuilder.Entity<UserTenant>()
            .HasOne(ut => ut.Tenant)
            .WithMany(t => t.UserTenants)
         .HasForeignKey(ut => ut.TenantId);
    }

}

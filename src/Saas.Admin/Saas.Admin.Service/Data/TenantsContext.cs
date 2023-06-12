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


        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.UserTenants)
            .WithOne()
            .HasForeignKey(t_fk => t_fk.TenantId)
            .IsRequired();

        modelBuilder.Entity<UserInfo>()
            .HasMany(u => u.UserTenants)
            .WithOne()
            .HasForeignKey(ut_fk => ut_fk.UserId)
            .IsRequired();
    }

}

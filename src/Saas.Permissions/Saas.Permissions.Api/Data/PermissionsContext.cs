namespace Saas.Permissions.Api.Data
{
    public class PermissionsContext : DbContext
    {
        public PermissionsContext(DbContextOptions<PermissionsContext> options)
            : base(options)
        {

        }

        public DbSet<Permission> Permissions { get; set; }

    }
}

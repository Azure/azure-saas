using Microsoft.EntityFrameworkCore;
using Saas.Domain.Models;

namespace Saas.Admin.Web.Models
{
    public partial class CatalogDbContext : DbContext
    {
        public CatalogDbContext()
        {
        }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Tenant> Tenants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenant");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(37);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

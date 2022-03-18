#nullable disable

namespace Saas.Admin.Service.Data;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.RoutePrefix).IsUnique(true);

        builder.Property(t => t.Name).IsRequired();

        builder.Property(t => t.IsCancelled).HasDefaultValue(false)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.IsProvisioned).HasDefaultValue(false)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.RoutePrefix).IsRequired();

        builder.Property(t => t.CreatedTime)
            .HasDefaultValue<DateTime?>(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ConcurrencyToken).IsConcurrencyToken();
    }
}

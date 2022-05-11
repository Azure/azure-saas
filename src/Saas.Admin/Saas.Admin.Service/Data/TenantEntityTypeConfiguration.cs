#nullable disable

namespace Saas.Admin.Service.Data;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Route).IsUnique(true);

        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Route).IsRequired();
        builder.Property(t => t.CreatorEmail).IsRequired();

        builder.Property(t => t.CreatedTime)
            .IsRequired()
            .HasDefaultValue<DateTime?>(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ConcurrencyToken)
            .IsConcurrencyToken();
    }
}

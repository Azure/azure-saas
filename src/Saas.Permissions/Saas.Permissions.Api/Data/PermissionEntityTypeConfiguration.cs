using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Saas.Permissions.Api.Data;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => new { p.UserId, p.TenantId, p.PermissionStr }).IsUnique();
    }
}

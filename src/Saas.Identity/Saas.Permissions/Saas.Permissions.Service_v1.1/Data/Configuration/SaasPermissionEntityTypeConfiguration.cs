using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saas.Identity.Authorization.Model.Data;

namespace Saas.Permissions.Service.Data.Configuration;

public class SaasPermissionEntityTypeConfiguration : IEntityTypeConfiguration<SaasPermission>
{
    public void Configure(EntityTypeBuilder<SaasPermission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => new { p.UserId, p.TenantId }).IsUnique();
    }
}

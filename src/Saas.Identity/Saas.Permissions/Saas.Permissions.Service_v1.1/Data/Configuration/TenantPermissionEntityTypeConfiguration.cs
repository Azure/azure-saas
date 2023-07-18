using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saas.Identity.Authorization.Model.Data;

namespace Saas.Permissions.Service.Data.Configuration;

public class TenantPermissionEntityTypeConfiguration : IEntityTypeConfiguration<TenantPermission>
{
    public void Configure(EntityTypeBuilder<TenantPermission> builder)
    {
        builder.HasKey(p => p.Id);
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saas.Identity.Authorization.Model.Data;

namespace Saas.Permissions.Service.Data.Configuration;

public class UserPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.HasKey(p => p.Id);
    }
}

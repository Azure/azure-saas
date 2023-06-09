
using Saas.Admin.Service.Data.Models.OnBoarding;

namespace Saas.Admin.Service.Data;

public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {

        builder.HasIndex(t => t.Route).IsUnique(true);

        builder.Property(t => t.CreatedDate)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.InitReady)
            .IsRequired()
            .HasDefaultValue(false)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ConcurrencyToken)
            .IsConcurrencyToken();
    }

    public void Configure(EntityTypeBuilder<UserTenant> builder)
    {

        builder.Property(t => t.ExpiryDate)
            .IsRequired()
            .HasDefaultValue(DateTime.UtcNow.AddDays(90)) //Default 90 days
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ExpiresAfter)
           .IsRequired()
           .HasDefaultValue(90) //Default 90 days
           .ValueGeneratedOnAdd();

        builder.Property(t => t.CreatedDate)
           .IsRequired()
           .HasDefaultValue(DateTime.UtcNow)
           .ValueGeneratedOnAdd();

        builder.Property(t => t.CCCode)
          .IsRequired()
          .HasDefaultValue("OO1")
          .ValueGeneratedOnAdd();

        builder.Property(t => t.EmpNo)
          .IsRequired()
          .HasDefaultValue("001") 
          .ValueGeneratedOnAdd();
    }
}

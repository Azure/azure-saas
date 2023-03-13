using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Identity.Authorization.Model.Data;

[Table("SaasPermissions")]
public record SaasPermission
{
    [Key]
    [Column("Id")]
    public int Id { get; init; }

    [Required]
    [Column("UserId")]
    public Guid UserId { get; init; }

    [Required]
    [Column("TenantId")]
    public Guid TenantId { get; init; }

    [ForeignKey("SaasPermissionId")]
    public ICollection<UserPermission> UserPermissions { get; init; } = null!;

    [ForeignKey("SaasPermissionId")]
    public ICollection<TenantPermission> TenantPermissions { get; init; } = null!;
}

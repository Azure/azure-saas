using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Saas.Identity.Authorization.Model.Data;
public abstract record PermissionBase
{

    [NotMapped]
    public abstract string EntityIdentifier { get; }

    [NotMapped]
    protected abstract Guid EntityId { get; }

    [Key]
    [Column("Id")]
    public int Id { get; init; }

    [Column("SaasPermissionId")]
    public int SaasPermissionId { get; init; }

    [Required]
    [Column("PermissionStr")]
    public string PermissionStr { get; init; } = null!;

    public string ToClaim()
    {
        return $"{EntityIdentifier}.{EntityId}.{PermissionStr}";
    }
}

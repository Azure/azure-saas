
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Admin.Service.Data.Models.OnBoarding;

[Table("Organization")]
/// <summary>
/// Represents a single registered organization/company
/// </summary>
public class Tenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Key]
    [Required]
    public Guid Guid { get; set; }


    /// <summary>
    /// Represents company name
    /// </summary>
    [Required]
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Represent an id to the industry/category this company falls into
    /// </summary>
    [Required]
    public int Industry { get; set; }

    [Required]
    public int ProductTierId { get; set; }

    [Required]
    public string Country { get; set; } = string.Empty;

    public int Employees { get; set; }

    [Required]
    public bool InitReady { get; set; } = false;

    public string? DatabaseName { get; set; }
    public bool ExternalDB { get; set; }

    public string TimeZone { get; set; } = "E. Africa Standard Time";

    public string? SqlServerRegion { get; set; }

    //User creator email
    [Required]
    public string CreatedUser { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedUser { get; set; }
    public DateTime UpdatedDate { get; set; }

    [Timestamp]
    public byte[]? ConcurrencyToken { get; set; }


    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();

}

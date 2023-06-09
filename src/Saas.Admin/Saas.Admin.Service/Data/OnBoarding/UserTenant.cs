
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Admin.Service.Data.Models.OnBoarding;


[Table("Employee")]
/// <summary>
/// A user can belong to zero or many tenants whilst a tenant can have many users
/// This model is used specifically for that, to hold information that relates a user to a tenant
/// </summary>
public class UserTenant
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string EmpNo { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public int ExpiresAfter { get; set; }

    /// <summary>
    /// Identifies whether the user is admin
    /// </summary>
    public bool SuperUser { get; set; }
   
    public string CCCode { get; set; } = string.Empty;

    public string RegSource { get; set; } = string.Empty;

    public string? Profession { get; set; }

    public bool PrincipalUser { get; set; }

    public string CreatedUser { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public Guid TenantId { get; set; }


    public Guid UserId { get; set; }

    [NotMapped]
    public Tenant Tenant { get; set; }

    [NotMapped]
    public UserInfo UserInfo { get; set; }
}



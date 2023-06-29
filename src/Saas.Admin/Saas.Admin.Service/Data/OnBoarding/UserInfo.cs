
using System.ComponentModel.DataAnnotations.Schema;

namespace Saas.Admin.Service.Data.Models.OnBoarding;

[Table("UserInfo")]
/// <summary>
/// Used to collect and hold information about any user registered with the system
/// </summary>
public class UserInfo
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Key]
    public Guid Guid { get; set; }

    /// <summary>
    /// user principal id, as provided by choosen identity provider,which is mostly an email
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string FullNames { get; set; } = string.Empty;


    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Security Question
    /// </summary>
    [Required]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Answer to the security question above
    /// </summary>
    [Required]
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// user's other email specified
    /// </summary>
    public string? Email { get; set; }

    public string Telephone { get; set; } = string.Empty;

    public int LockAfter { get; set; }

    public string? BioUserID { get; set; }

    public DateTime DOB { get; set; }
    public string? IDType { get; set; }

    public bool AcceptTerms { get; set; }
    public bool Notifications { get; set; }
    public DateTime? CreatedDate { get; set; }

    public ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
}

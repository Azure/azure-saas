namespace Saas.SignupAdministration.Web.Models;

public class SadUser
{
    public long Id { get; set; }
    /// <summary>
    /// user principal id, as provided by choosen identity provider,which is mostly an email
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    public string FullNames { get; set; } = string.Empty;

    public string EmpNo { get; set; } = string.Empty;

    public string Password { get; set; } = "0";

    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Security Question
    /// </summary>
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Answer to the security question above
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// user's other email specified
    /// </summary>
    public string Email { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;   

    public DateOnly  ExpiryDate { get;set;} 

    public int ExpiresAfter { get;}

    public int LockAfter { get; set; }

    public bool ImmediateChange { get; set; }

    public bool IsActive { get; set; }


    /// <summary>
    /// Identifies whether the user is admin
    /// </summary>
    public bool SuperUser { get; set; }


    public string BioUserID { get; set; } = string.Empty;

    public string CCCode { get; set; } = string.Empty;

    public string RegSource { get; set; } = string.Empty;
    public DateOnly DOB { get; set; }
    public string IDType { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public int Employees { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; } 
    public bool Notifications { get; set; }
   
    public bool InitReady { get; set; }
    public bool ExternalDB { get; set; }
    public bool PrincipalUser { get; set; }
    public string TimeZone { get; set; } = string.Empty;

    public string CreatedUser { get; set; } = string.Empty;

    public DateOnly CreatedDate { get; set; }

    public string UpdatedUser { get; set; } = string.Empty;
    public DateOnly UpdatedDate { get; set; }

    public string Terminus { get; set; } = string.Empty;

    public string Narration { get; set; } = string.Empty;

    public string DBIdentity { get; set; } = string.Empty;

}

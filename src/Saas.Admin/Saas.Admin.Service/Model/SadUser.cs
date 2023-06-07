using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

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

    [JsonIgnore]
    public string Password { get; set; } = "0";

    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Security Question
    /// </summary>
    [JsonIgnore]
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Answer to the security question above
    /// </summary>
    [JsonIgnore]
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// user's other email specified
    /// </summary>
    public string? Email { get; set; }

    public string Telephone { get; set; } = string.Empty;

    [JsonIgnore]
    public DateOnly  ExpiryDate { get;set;}

    [JsonIgnore]
    public int ExpiresAfter { get; set; }

    [JsonIgnore]
    public int LockAfter { get; set; }

    [JsonIgnore]
    public bool ImmediateChange { get; set; }

    public bool IsActive { get; set; }


    /// <summary>
    /// Identifies whether the user is admin
    /// </summary>
    public bool SuperUser { get; set; }

    [JsonIgnore]
    public string BioUserID { get; set; } = string.Empty;

    [JsonIgnore]
    public string CCCode { get; set; } = string.Empty;

    [JsonIgnore]
    public string RegSource { get; set; } = string.Empty;
    [JsonIgnore]
    public DateTime DOB { get; set; }
    public string IDType { get; set; } = string.Empty;
    public string? Profession { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public int Employees { get; set; }
    public string Country { get; set; } = string.Empty;
    public bool AcceptTerms { get; set; } 
    public bool Notifications { get; set; }
    [JsonIgnore]
    public bool InitReady { get; set; }
    [JsonIgnore]
    public bool ExternalDB { get; set; }
    [JsonIgnore]
    public bool PrincipalUser { get; set; }
    public string TimeZone { get; set; } = string.Empty;

    [JsonIgnore]
    public string CreatedUser { get; set; } = string.Empty;

    [JsonIgnore]
    public DateOnly CreatedDate { get; set; }

    [JsonIgnore]
    public string? UpdatedUser { get; set; }
    public DateOnly UpdatedDate { get; set; }

    [JsonIgnore]
    public string Terminus { get; set; } = string.Empty;

   
    public string? Narration { get; set; }

    [JsonIgnore]
    public string DBIdentity { get; set; } = string.Empty;

    /// <summary>
    /// Used to generate a hashed password for this user
    /// Mostly used when saving this user's password to database
    /// or when comparing user's password against one stored in the database during sign in
    /// </summary>
    /// <param name="plain">password in plain text</param>
    /// <param name="saltString">string text used as salt for this hashing algorithm</param>
    /// <returns>A hashed version of the plain text provided</returns>
    public static string passwordHash(string plain, string saltString)
    {

        // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
        byte[] salt = Encoding.UTF8.GetBytes(saltString);
        using (RandomNumberGenerator rngCsp = RandomNumberGenerator.Create())
        {
            rngCsp.GetNonZeroBytes(salt);
        }

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = BitConverter.ToString(KeyDerivation.Pbkdf2(
            password: plain,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 350000,
            numBytesRequested: 64)).Replace("-", "");

        return hashed;
    }
}

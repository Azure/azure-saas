
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Saas.Admin.Service.Data.Models.OnBoarding;

[Table("User")]
/// <summary>
/// Used to collect and hold information about any user registered with the system
/// </summary>
public class UserInfo
{
    private string password = string.Empty;
    private string confirmPassword = string.Empty;
    private string answer = string.Empty;


    [NotMapped]
    public static string salt;

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

    public string Password
    {
        get { return password; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0"; 
            }
            password = passwordHash(value, salt);
        }
    }

    public string ConfirmPassword {
        get { return confirmPassword; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
            }
            confirmPassword = passwordHash(value, salt);
        }
    }

    /// <summary>
    /// Security Question
    /// </summary>
    [Required] 
    public string Question { get; set; } = string.Empty;

    /// <summary>
    /// Answer to the security question above
    /// </summary>
    [Required] 
    public string Answer {
        get { return answer; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "0";
            }
            answer = passwordHash(value, salt);
        }
    }

    /// <summary>
    /// user's other email specified
    /// </summary>
    public string? Email { get; set; }

    public string Telephone { get; set; } = string.Empty;

    public int LockAfter { get; set; }

    public string BioUserID { get; set; } = string.Empty;

    public DateTime DOB { get; set; }
    public string IDType { get; set; } = string.Empty;
    public string? Profession { get; set; }
    public bool AcceptTerms { get; set; }
    public bool Notifications { get; set; }
    public DateTime CreatedDate { get; set; }

    public ICollection<UserTenant> UserTenants { get; set; }

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

        // derive a 256-bit subkey (use HMACSHA256 with 350,000 iterations)
        string hashed = BitConverter.ToString(KeyDerivation.Pbkdf2(
            password: plain,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 350000,
            numBytesRequested: 64)).Replace("-", "");

        return hashed;
    }
}

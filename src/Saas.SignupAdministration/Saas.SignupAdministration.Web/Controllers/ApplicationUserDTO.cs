namespace Saas.SignupAdministration.Web.Controllers;

public class ApplicationUserDTO
{
    public string Company { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public  string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public long Id { get; set; }
    public string Industry { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

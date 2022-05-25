namespace Saas.Application.Web.Models;

public class AppSettings : IAdminClientSettings
{
    public string RedirectUri { get; set; }
    public string AdminServiceBaseUrl { get; set; }
    public string AdminServiceScopes { get; set; }
}

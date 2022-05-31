namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public class UserViewModel
{
    public string UserId { get; internal set; } = string.Empty;
    public string DisplayName { get; internal set; } = string.Empty;
    public string Permissions { get; internal set; } = string.Empty;
}

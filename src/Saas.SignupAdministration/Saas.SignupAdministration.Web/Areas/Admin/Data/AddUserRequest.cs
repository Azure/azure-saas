namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public class AddUserRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string ConfirmUserEmail { get; set; } = string.Empty;
}

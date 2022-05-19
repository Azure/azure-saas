namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public class AddUserRequest
{
    public string TenantId { get; set; }
    public string UserEmail { get; set; }
    public string ConfirmUserEmail { get; set; }
    //public string Permission { get; set; }
}

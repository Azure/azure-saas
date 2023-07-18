namespace Saas.SignupAdministration.Web.Models;

public class ErrorViewModel
{
    public string RequestId { get; set; } = string.Empty;

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

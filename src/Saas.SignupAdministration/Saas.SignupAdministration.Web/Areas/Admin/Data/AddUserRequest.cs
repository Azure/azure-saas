namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public record AddUserRequest
{
    public string TenantId { get; init; } = null!;
    public string UserEmail { get; init; } = null!;
    public string ConfirmUserEmail { get; init; } = null!;
}

namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public record UserViewModel
{
    public Guid UserId { get; internal init; }
    public string DisplayName { get; internal init; } = null!;
    public string Permissions { get; internal init; } = null!;
}

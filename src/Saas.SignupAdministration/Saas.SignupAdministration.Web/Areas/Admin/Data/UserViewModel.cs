using Saas.Admin.Client;

namespace Saas.SignupAdministration.Web.Areas.Admin.Data;

public class UserViewModel
{
    public Guid UserId { get; internal init; }
    public string DisplayName { get; internal init; } = null!;
    public string Permissions { get; internal init; } = null!;

    public UserViewModel(UserDTO userDTO, string permissions)
    {
        UserId = userDTO.UserId;
        DisplayName = userDTO.DisplayName;
        Permissions = permissions;
    }
}

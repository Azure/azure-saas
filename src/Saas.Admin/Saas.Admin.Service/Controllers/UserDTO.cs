namespace Saas.Admin.Service.Controllers;


public class UserDTO
{
    public UserDTO(Guid userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }
    public Guid UserId { get; set; }

    public string DisplayName { get; set; }
}
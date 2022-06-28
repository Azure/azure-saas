namespace Saas.Billing.Service.Controllers.DTOs;


public class UserDTO
{
    public UserDTO(string userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }
    public string UserId { get; set; }

    public string DisplayName { get; set; }
}
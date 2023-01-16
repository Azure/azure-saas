namespace Saas.Permissions.Service.Models;

public record User
{
    public string? UserId { get; init; }
    public string? DisplayName { get; init; }  
}

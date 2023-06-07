namespace Saas.Admin.Service.Model;

public class AppUser
{
    public string? UserId { get; init; }
    public string? DisplayName { get; init; }
    public string? GivenName { get; set; }

    public string? Surname { get; set; }
    public string? Mail { get; set; }
}

namespace Saas.Permissions.Service.Models;


public record PermissionsClaimResponse
{
    public string[]? Permissions { get; init; }
}


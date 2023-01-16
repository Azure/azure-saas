namespace Saas.Permissions.Service.Models;



public record UnauthorizedResponse
{
    public UnauthorizedResponse(string _error)
    {
        Error = _error;
    }
    public string Error { get; init; }
}

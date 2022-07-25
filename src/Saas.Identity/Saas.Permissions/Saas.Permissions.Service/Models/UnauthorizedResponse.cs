namespace Saas.Permissions.Service.Models;



public class UnauthorizedResponse
{
    public UnauthorizedResponse(string _error)
    {
        Error = _error;
    }
    public string Error { get; set; }
}

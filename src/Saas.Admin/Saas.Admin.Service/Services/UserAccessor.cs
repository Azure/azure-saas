using Microsoft.Graph;
using Saas.Admin.Service.Interfaces;
using Saas.Admin.Service.Model;
using Saas.SignupAdministration.Web.Models;
using System.Security.Claims;

namespace Saas.Admin.Service.Services;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public SadUser GetUser(SadUser sadUser)
    {
  

        if(_httpContextAccessor.HttpContext != null)
        {
            sadUser.UserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            sadUser.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            sadUser.FullNames = _httpContextAccessor.HttpContext.User.FindFirstValue("name") ?? string.Empty;
            sadUser.Telephone = _httpContextAccessor.HttpContext.User.FindFirstValue("telephone") ?? string.Empty;
            sadUser.Country = _httpContextAccessor.HttpContext.User.FindFirstValue("country") ?? string.Empty;
            sadUser.Industry = _httpContextAccessor.HttpContext.User.FindFirstValue("industry") ?? string.Empty;
            sadUser.Employees = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("noOfEmployees") ?? "0");
            sadUser.Terminus = "001";
        }

        return sadUser;
    }

    public ISadUserDto GetUser()
    {
        ISadUserDto sadUser = new SadUserDto();

        if (_httpContextAccessor.HttpContext != null)
        {
            sadUser.UserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            sadUser.Email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            sadUser.FullNames = _httpContextAccessor.HttpContext.User.FindFirstValue("name") ?? string.Empty;
            sadUser.Telephone = _httpContextAccessor.HttpContext.User.FindFirstValue("telephone") ?? string.Empty;
            sadUser.Country = _httpContextAccessor.HttpContext.User.FindFirstValue("country") ?? string.Empty;
            sadUser.Industry = _httpContextAccessor.HttpContext.User.FindFirstValue("industry") ?? string.Empty;
 
        }

        return sadUser;
    }
}

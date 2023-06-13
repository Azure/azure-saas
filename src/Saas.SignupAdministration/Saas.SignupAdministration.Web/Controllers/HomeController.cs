using Microsoft.AspNetCore.Antiforgery;

//Saas permission
using Saas.Identity.Authorization.Model.Claim;

namespace Saas.SignupAdministration.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{

    //User information and token acquistation added to facilitate REST API 
    private readonly IApplicationUser _applicationUser;
    private readonly IAntiforgery _antiforgery;



    public HomeController( IApplicationUser applicationUser,  IAntiforgery antiforgery)
    {
        _applicationUser = applicationUser;
        _antiforgery = antiforgery;
    }


    [HttpGet]
    public IActionResult Index()
    {

        if (User?.Identity?.IsAuthenticated ?? false)
        {
            bool isRegistered = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;
            if (isRegistered)
            {
                return Redirect("/dashboard");
            }

            return Redirect("/onboarding");

        }

        return Redirect("/");

    }


    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //public IActionResult Error()
    //{
    //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //}

    /// <summary>
    /// Temporary
    /// </summary>
    /// <returns>A json body containing user information including token</returns>
    [HttpGet("api/GetAuthUser")]
    public IActionResult GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            string? xsrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
            bool isAdmin = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;

            return new JsonResult(new { _applicationUser.GivenName, xsrf_token, isAdmin});

        }
        else
        {
            return Unauthorized();
        }

    }

}

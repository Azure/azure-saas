
using System.Diagnostics;
using Microsoft.AspNetCore.Antiforgery;


//Saas claims handling
using Saas.Shared.Options;

//Saas permission
using Saas.Identity.Authorization.Model.Claim;

namespace Saas.SignupAdministration.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{

    //User information and token acquistation added to facilitate REST API 
    private readonly IApplicationUser _applicationUser;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IAntiforgery _antiforgery;

    //Configured access scopes
    private readonly IEnumerable<string> _scopes;


    public HomeController( IApplicationUser applicationUser, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes, IAntiforgery antiforgery)
    {
        _applicationUser = applicationUser;
        _tokenAcquisition = tokenAcquisition;
        _scopes = scopes.Value.Scopes ?? throw new ArgumentNullException($"Scopes must be defined.");
        _antiforgery = antiforgery;
    }

    [HttpGet]
    public IActionResult Help()
    {
        return View();
    }



    [HttpGet]
    public IActionResult Index()
    {

        if (User?.Identity?.IsAuthenticated ?? false)
        {
            bool isRegistered = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;
            if (isRegistered)
            {
                return Redirect("https://localhost:3000/dashboard");
            }

            return Redirect("https://localhost:3000/onboarding");

        }

        return Redirect("https://localhost:3000");

    }

    [HttpGet]
    public IActionResult Pricing()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Temporary
    /// </summary>
    /// <returns>A json body containing user information including token</returns>
    [HttpGet("api/GetAuthUser")]
    public async Task<IActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            string? xsrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
            bool hassaas = User?.Claims?.HasSaasUserPermissionSelf() ?? false;
            bool isAdmin = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);

            return new JsonResult(new { _applicationUser.GivenName, accessToken, xsrf_token, hassaas, isAdmin });

        }
        else
        {
            return Unauthorized();
        }

    }

}

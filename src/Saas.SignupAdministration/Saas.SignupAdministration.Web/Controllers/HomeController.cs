
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;


//Saas claims handling
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Interfaces;

namespace Saas.SignupAdministration.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //User information and token acquistation added to facilitate REST API 
    private readonly IApplicationUser _applicationUser;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IAntiforgery _antiforgery;
    private readonly IDBServices _dbServices;

    //Configured access scopes
    private readonly IEnumerable<string> _scopes;


    public HomeController(ILogger<HomeController> logger, IApplicationUser applicationUser, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes, IAntiforgery antiforgery, IDBServices dbServices)
    {
        _logger = logger;
        _applicationUser = applicationUser;
        _tokenAcquisition = tokenAcquisition;
        _scopes = scopes.Value.Scopes ?? throw new ArgumentNullException($"Scopes must be defined.");
        _antiforgery = antiforgery;
        _dbServices = dbServices;
    }

    [HttpGet]
    public IActionResult Help()
    {
        return View();
    }
r


    [HttpGet]
    //[HttpPost]
    public async Task<IActionResult> Index()
    {


        if (User?.Identity?.IsAuthenticated ?? false)
        {
            bool isRegistered = await isUserRegistered();
            if (isRegistered)
            {
                return Redirect("https://localhost:3000/dashboard");
            }

            return Redirect("https://localhost:3000/onboarding");

        }

        return Redirect("https://localhost:3000");

        //return View();

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
    /// Replaced Index page to return basic user information used by index page to display
    /// information about logged in user
    /// It must be accessed by a logged in user
    /// </summary>
    /// <returns>A json body containing user information including token</returns>
    [HttpGet("api/user-info")]
    public async Task<IActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            string? xsrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
            //bool isAdmin = (User?.Claims?.HasSaasUserPermissionSelf() ?? false) && (User?.Claims?.HasSaasTenantPermissionAdmin() ?? false);
            bool isRegistered = await isUserRegistered();
            string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);

            return new JsonResult(new { _applicationUser.GivenName, isRegistered, accessToken, xsrf_token });

        }
        else
        {
            return Unauthorized();
        }

    }


    private async Task<bool> isUserRegistered()
    {

        string email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

        bool isRegistered = await _dbServices.isUserRegistered(email);

        return isRegistered;

    }

}

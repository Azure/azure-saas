using Microsoft.AspNetCore.Antiforgery;

//Saas permission
using Saas.Identity.Authorization.Model.Claim;
using System.Net.Mime;

namespace Saas.SignupAdministration.Web.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[Action]")]
public class HomeController : Controller
{

    //User information and token acquistation added to facilitate REST API 
    private readonly IApplicationUser _applicationUser;
    private readonly IAntiforgery _antiforgery;
    private readonly IConfiguration _configuration;
    private readonly string baseUrl;

    public HomeController( IApplicationUser applicationUser,  IAntiforgery antiforgery, IConfiguration configuration)
    {
        _applicationUser = applicationUser;
        _antiforgery = antiforgery;
        _configuration = configuration;
        baseUrl = _configuration.GetSection("AppSettings:developmentUrl").Value;
    }

    [HttpGet]
    [HttpPost]
    [Route("/account/logout")]
    public IActionResult Logout()
    {
        return Redirect(baseUrl);
    }

    [HttpGet("/")]
    public IActionResult Index()
    {

        if (User?.Identity?.IsAuthenticated ?? false)
        {
            bool isRegistered = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;
            if (isRegistered)
            {
                return Redirect($"{baseUrl}/dashboard");
            }

            return Redirect($"{baseUrl}/onboarding");

        }

        return Redirect($"{baseUrl}/dashboard");

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
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetUser()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            string? xsrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
            bool isRegistered = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;

            ApplicationUserDTO user = new ApplicationUserDTO
            { 
                Email = _applicationUser.EmailAddress,
                Telephone = _applicationUser.Telephone,
                Country = _applicationUser.Country,
                FullName = _applicationUser.FullName,
                Industry = _applicationUser.Industry,
                UserName = _applicationUser.EmailAddress

            };

            return new JsonResult(new { user,isRegistered });

        }
        else
        {
            return Unauthorized();
        }

    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCsrfToken()
    {
        string? csrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
        string cookie = _antiforgery.GetTokens(HttpContext).CookieToken
            ?? throw new BadHttpRequestException("Anti-forgery cookie cannot be null");

        HttpContext.Response.Cookies.Append(
            SR.CookieName, cookie,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/", // Set the desired cookie path
                SameSite = SameSiteMode.Lax
            }
            );

        return new JsonResult(new {token = csrf_token});

    }

}

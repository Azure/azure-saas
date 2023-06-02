using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Data.SqlClient;

//Saas claims handling
using Saas.Identity.Authorization.Model.Claim;
using Saas.Shared.Options;

namespace Saas.SignupAdministration.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //User information and token acquistation added to facilitate REST API 
    private readonly IApplicationUser _applicationUser;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IAntiforgery _antiforgery;

    //Configured access scopes
    private readonly IEnumerable<string> _scopes;


    public HomeController(ILogger<HomeController> logger, IApplicationUser applicationUser, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes, IAntiforgery antiforgery)
    {
        _logger = logger;
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
    [HttpPost]
    public IActionResult Index()
    {
<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
        //if (user.identity.isauthenticated)
        //{
        //    return redirect("https://localhost:3000/onboarding");
        //}
<<<<<<< Updated upstream
        return Redirect("https://192.168.1.13:3000");
=======
        //return Redirect("https://192.168.1.13:3000");
>>>>>>> Stashed changes
        return View();
>>>>>>> Stashed changes
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
    [HttpGet("user-info")]
    public async Task<IActionResult> Test()
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

        string constr = "Server=tcp:sqldb-asdk-dev-lsg5.database.windows.net,1433;Initial Catalog=iBusinessSaasTests;Persist Security Info=False;User ID=sqlAdmin;Password=UnK307r8DUW0zvlOjli4d1SW+wB138HNA4xgA/oPLe/uA2zmrXzh1NspSEBWM8W6;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //Connect to database. then add user
        using (SqlConnection con = new SqlConnection(constr))
        {
            await con.OpenAsync();

            int flag = 0;

            using (SqlCommand command = new SqlCommand("spCheckIfUserExists", con))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("UserName", SqlDbType.NVarChar).Value = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

                

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {

                    while (reader.Read())
                    {
                        flag = reader.GetInt32(0);
                    }

                }

            }
            bool isRegistered = flag == 1 ? true : false;
            return isRegistered;

        }

    }

}

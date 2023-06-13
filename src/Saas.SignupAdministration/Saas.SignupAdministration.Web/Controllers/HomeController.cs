﻿using Microsoft.AspNetCore.Antiforgery;

//Saas permission
using Saas.Identity.Authorization.Model.Claim;
using System.Net.Mime;

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
                return Redirect("https://localhost:3000/dashboard");
            }

            return Redirect("https://localhost:3000/onboarding");

        }

        return Redirect("https://localhost:3000/");

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
    [HttpGet("/api/user")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            string? xsrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;
            bool hasTenants = User?.Claims?.HasSaasTenantPermissionAdmin() ?? false;

            ApplicationUserDTO user = new ApplicationUserDTO
            { 
                Email = _applicationUser.EmailAddress,
                Telephone = _applicationUser.Telephone,
                Country = _applicationUser.Country,
                FullName = _applicationUser.FullName,
                Industry = _applicationUser.Industry,
                UserName = _applicationUser.EmailAddress

            };

            return new JsonResult(new { user, xsrf_token, hasTenants});

        }
        else
        {
            return Unauthorized();
        }

    }

    [HttpGet("/get-csrf-token")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetCsrfToken()
    {
        string? csrf_token = _antiforgery.GetTokens(HttpContext).RequestToken;

        return new JsonResult(new {token = csrf_token});

    }

}

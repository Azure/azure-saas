using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas.Admin.Service.Data;
using Saas.Admin.Service.Interfaces;
using Saas.SignupAdministration.Web.Models;
using System.Net.Mime;
using System.Security.Claims;

namespace Saas.Admin.Service.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SadUserController : ControllerBase
{
    private readonly ISadUserService _sadUserService;
    

    public SadUserController(ISadUserService sadUserService)
    {
        _sadUserService = sadUserService;
       
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SadUser), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> OnboardTen(SadUser admin)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new Exception("Error processing you request");


            //SadUser user = await getUserinfo(admin);
            //Normalize user email
            admin.UserName = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            admin.Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            admin.FullNames = User.FindFirst("name")?.Value ?? string.Empty;
            admin.Telephone = User.FindFirst("telephone")?.Value ?? string.Empty;
            admin.Country = User.FindFirst("country")?.Value ?? string.Empty;
            admin.Industry = User.FindFirst("industry")?.Value ?? string.Empty;
            admin.Employees = int.Parse(User.FindFirst("noOfEmployees")?.Value ?? "0");
            admin.Terminus = "001";

            admin = await _sadUserService.AddSadUser(admin, 0);

            if(admin.Id == 0)//User exists
            {
                return Conflict(new {message = "User already exists"});
            }
                

            //return Ok(/*new { userId = createdUser.Id, createdUser, message = "success" }*/);
            return CreatedAtAction(nameof(OnboardTen), new { userId = admin.Id }, admin);

        }
        catch (DbUpdateException ex)
        {
            return BadRequest((ex.InnerException ?? ex).Message);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet("/api/user")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult PreOnboard()
    {
        //Get userinfo from claims, then return
        var userInfo = new
        {
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Telephone = User.FindFirst("telephone")?.Value ?? string.Empty,
            Country = User.FindFirst("country")?.Value ?? string.Empty,
            Industry = User.FindFirst("industry")?.Value ?? string.Empty,
            OrganizationName = User.FindFirst("organizationName")?.Value ?? string.Empty,
            NoOfEmployees = int.Parse(User.FindFirst("noOfEmployees")?.Value ?? "0"),
            Name = User.FindFirst("name")?.Value ?? string.Empty,
        };


        return Ok(userInfo);

    }


     // <summary>
    // Update user information from graph
    // </summary>
    //private async Task<SadUser> getUserinfo(SadUser admin)
    //{
    //    string email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        

    //    if (string.IsNullOrEmpty(email))
    //    {
    //        throw new ArgumentNullException("User principal name does not exists");
    //    }

    //    //Add this user email as username
    //    admin.UserName = email;

    //    SadUser user =await  _graphservices.GetUser(email);

    //    admin.FullNames = user.FullNames;
    //    admin.Email = user.Email;
    //    admin.Telephone = user.Telephone;
    //    admin.RegSource = user.RegSource;
    //    //admin.Country = user.Country;

    //    //Add user terminal before exiting
    //    admin.Terminus = HttpContext.Connection.RemoteIpAddress?.ToString()??"Not captured";

    //    return admin;

    //}
}

using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saas.Admin.Service.Data;
using Saas.SignupAdministration.Web.Models;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;

namespace Saas.Admin.Service.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SadUserController : ControllerBase
{
    public readonly ISadUserService _sadUserService;

    public SadUserController(ISadUserService sadUserService)
    {
        _sadUserService = sadUserService;
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    //[ProducesResponseType(typeof(SadUser), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OnboardTen(SadUser admin)
    {

        try
        {
            if (!ModelState.IsValid)
                throw new Exception("Error processing you request");

            updateUserinfo(admin);
            //SadUser createdUser = await _sadUserService.AddSadUser(admin, 0);
            return Ok(/*new { userId = createdUser.Id, createdUser, message = "success" }*/);
            //return CreatedAtAction("dashboard", new { userId = createdUser.Id }, createdUser);

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

    /// <summary>
    /// Update user information from graph
    /// </summary>
    private async Task updateUserinfo(SadUser admin)
    {
        
        foreach (var item in User.Claims)
        {
            Console.WriteLine($"{item.Type} => {item.Value}");
            
        }
        Console.WriteLine(User.Identity.Name);
        admin.Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        admin.FullNames = User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        admin.UserName = User.FindFirst(ClaimTypes.Upn)?.Value ?? admin.Email;

    }
}

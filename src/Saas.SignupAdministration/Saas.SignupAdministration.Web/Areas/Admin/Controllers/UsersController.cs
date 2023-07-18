using Saas.Admin.Client;

namespace Saas.SignupAdministration.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Controller]
[Route("[area]/tenants/{tenantid}/users")]
[Authorize]
// [AuthorizeForScopes(Scopes = new string[] { "tenant.read", "tenant.global.read", "tenant.write", "tenant.global.write", "tenant.delete", "tenant.global.delete" })]
public class UsersController : Controller
{
    private readonly IAdminServiceClient _adminServiceClient;

    public UsersController(IAdminServiceClient adminServiceClient)
    {
        _adminServiceClient = adminServiceClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid tenantid)
    {
        var users = await _adminServiceClient.UsersAsync(tenantid);
        var userViewModels = await users
            .ToAsyncEnumerable()
            .SelectAwait(async x => new UserViewModel
            {
                DisplayName = x.DisplayName,
                UserId = x.UserId,
                Permissions = string.Join(", ", await _adminServiceClient.PermissionsAllAsync(tenantid, x.UserId))
            }).ToListAsync();
        ViewData["tenantid"] = tenantid;
        return View(userViewModels);
    }

    [HttpGet]
    [Route("AddUserToTenant", Name = "AddUserToTenant")]
    public IActionResult AddUserToTenant(string tenantId)
    {
        return View(new AddUserRequest { TenantId = tenantId });
    }

    // POST: Admin/Tenants/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("AddUserToTenant")]
    public async Task<IActionResult> AddUserToTenant(Guid tenantid, [Bind("TenantId, UserEmail, ConfirmUserEmail")] AddUserRequest addUserRequest)
    {
        if (string.Compare(tenantid.ToString(), addUserRequest.TenantId) != 0)
        {
            return NotFound();
        }

        if (!Guid.TryParse(addUserRequest.TenantId, out var userTenantId)) 
        {
            throw new ArgumentException($"The added user tenant id value is invalid '{addUserRequest.TenantId}'. Vakue must be a guid. ");
        }

        if (ModelState.IsValid 
            && string.Compare(addUserRequest.UserEmail, addUserRequest.ConfirmUserEmail) == 0)
        {
            try
            {
                await _adminServiceClient.InviteAsync(userTenantId, addUserRequest.UserEmail);
            }
            catch (ApiException)
            {
                return NotFound();
            }
            return RedirectToAction(
                "Index", 
                new 
                { 
                    area = "Admin", 
                    controller = "users", 
                    tenantid = addUserRequest.TenantId 
                });
        }
        return View(addUserRequest);
    }
}

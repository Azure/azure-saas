using Saas.Admin.Client;

namespace Saas.SignupAdministration.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Controller]
[Route("[area]/Tenants/{tenantid}/Users")]
[Authorize]
// [AuthorizeForScopes(Scopes = new string[] { "tenant.read", "tenant.global.read", "tenant.write", "tenant.global.write", "tenant.delete", "tenant.global.delete" })]
public class UsersController : Controller
{
    private readonly IAdminServiceClient _adminServiceClient;

    public UsersController(IAdminServiceClient adminServiceClient)
    {
        _adminServiceClient = adminServiceClient;
    }

    // GET: Admin/Tenants/{tenantid}/Users
    [HttpGet]
    public async Task<IActionResult> Index(Guid tenantid)
    {
        var users = await _adminServiceClient.UsersAsync(tenantid);
        var userViewModels = await users
            .ToAsyncEnumerable()
            .SelectAwait(async x => new UserViewModel
            (
                x,
                permissions: string.Join(", ", await _adminServiceClient.PermissionsAllAsync(tenantid, x.UserId))
            )).ToListAsync();
        ViewData["tenantid"] = tenantid;
        return View(userViewModels);
    }

    // GET: Admin/Tenants/{tenantid}/Users/AddUserToTenant
    [HttpGet]
    [Route("AddUserToTenant", Name = "AddUserToTenant")]
    public IActionResult AddUserToTenant(string tenantId)
    {
        return View(new AddUserRequest { TenantId = tenantId });
    }

    // POST: Admin/Tenants/{tenantid}/Users/AddUserToTenant
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("AddUserToTenant")]
    public async Task<IActionResult> AddUserToTenant(string tenantid, [Bind("TenantId, UserEmail, ConfirmUserEmail")] AddUserRequest addUserRequest)
    {
        if (string.Compare(tenantid, addUserRequest.TenantId) != 0)
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

    [HttpGet]
    [Route("Edit/{id}", Name = "Edit")]
    // GET: Admin/Tenants/{tenantid}/Users/Edit/7
    public async Task<IActionResult> Edit(string tenantId, string id)
    {
        if (tenantId == null || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return NotFound();
        }
        if (id == null || !Guid.TryParse(id, out var userGuid))
        {
            return NotFound();
        }

        var user = await _adminServiceClient.UserAsync(tenantGuid, userGuid);

        ViewData["tenantid"] = tenantId;
        return user == null
            ? (IActionResult)NotFound()
            : View(new UserViewModel(
                user,
                null // do not show permissions ==> not implemented here (yet)
            ));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(string tenantid, string id, [Bind("Id,DisplayName")] UserDTO user)
    {
        // TODO not implemented
        return NotFound();
    }

    [HttpGet]
    [Route("Details/{id}", Name = "Details")]
    // GET: Admin/Tenants/{tenantid}/Users/Details/7
    public async Task<IActionResult> Details(string tenantId, string id)
    {
        if (tenantId == null || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return NotFound();
        }
        if (id == null || !Guid.TryParse(id, out var userGuid))
        {
            return NotFound();
        }

        var user = await _adminServiceClient.UserAsync(tenantGuid, userGuid);

        ViewData["tenantid"] = tenantId;
        return user == null
            ? (IActionResult)NotFound()
            : View(new UserViewModel(
                user,
                null // do not show permissions ==> not implemented here (yet)
            ));
    }

    [HttpGet]
    [Route("Delete/{id}", Name = "Delete")]
    // GET: Admin/Tenants/{tenantid}/Users/Delete/7
    public async Task<IActionResult> Delete(string tenantId, string id)
    {
        if (tenantId == null || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return NotFound();
        }
        if (id == null || !Guid.TryParse(id, out var userGuid))
        {
            return NotFound();
        }

        var user = await _adminServiceClient.UserAsync(tenantGuid, userGuid);

        ViewData["tenantid"] = tenantId;
        return View(user);
    }

    // POST: Admin/Tenants/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string tenantId, string id)
    {
        if (tenantId == null || !Guid.TryParse(tenantId, out var tenantGuid))
        {
            return NotFound();
        }
        if (id == null || !Guid.TryParse(id, out var userGuid))
        {
            return NotFound();
        }

        // TODO not implemented
        throw new NotImplementedException();
        //await _adminServiceClient.UserDeleteAsync(tenantGuid, userGuid);
        return RedirectToAction(nameof(Index));
    }
}

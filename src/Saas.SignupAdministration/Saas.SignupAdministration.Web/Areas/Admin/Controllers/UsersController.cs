namespace Saas.SignupAdministration.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Controller]
[Route("[area]/tenants/{tenantid}/users")]
public class UsersController : Controller
{
    private readonly IAdminServiceClient _adminServiceClient;

    public UsersController(IAdminServiceClient adminServiceClient)
    {
        _adminServiceClient = adminServiceClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tenantid)
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

        return View(userViewModels);
    }

    [HttpGet]
    [Route("AddUserToTenant")]
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
    public async Task<IActionResult> AddUserToTenant(string tenantid, [Bind("TenantId, UserEmail, ConfirmUserEmail")] AddUserRequest addUserRequest)
    {
        Guid guid = new Guid();
        if (!Guid.TryParse(tenantid, out guid) || string.Compare(tenantid, addUserRequest.TenantId) != 0)
        {
            return NotFound();
        }

        if (ModelState.IsValid && string.Compare(addUserRequest.UserEmail, addUserRequest.ConfirmUserEmail)==0)
        {
            try
            {
                await _adminServiceClient.PermissionsPOSTAsync(addUserRequest.TenantId, addUserRequest.UserEmail, new List<string> { "TenantAdmin" });
            }
            catch (ApiException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(addUserRequest);
    }
}

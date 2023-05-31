using Microsoft.CodeAnalysis;
using Saas.SignupAdministration.Web.Services.StateMachine;

namespace Saas.SignupAdministration.Web.Controllers;

[Authorize()]
// [AuthorizeForScopes(Scopes = new string[] { "tenant.read", "tenant.global.read", "tenant.write", "tenant.global.write", "tenant.delete", "tenant.global.delete" })]
public class OnboardingWorkflowController : Controller
{
    private readonly ILogger<OnboardingWorkflowController> _logger;
    private readonly OnboardingWorkflowService _onboardingWorkflow;

    public OnboardingWorkflowController(ILogger<OnboardingWorkflowController> logger, OnboardingWorkflowService onboardingWorkflow)
    {
        _logger = logger;
        _onboardingWorkflow = onboardingWorkflow;
    }

    /// <summary>
    /// This methods handles all onboarding process on the go
    /// The other models are listed after the methods
    /// </summary>
    /// <param name="organization">Organization model</param>
    /// <returns> An appropriate result based on given data</returns>
    [HttpPost("/onboarding")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleBatchRegistration([FromBody] Organization organization)
    {
        if (!ModelState.IsValid) //Return a bad request
            return BadRequest("Cannot process your request");

        //step 1
        _onboardingWorkflow.OnboardingWorkflowItem.OrganizationName = organization.OrganizationName;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnOrganizationNamePosted);

        //step 2
        _onboardingWorkflow.OnboardingWorkflowItem.CategoryId = organization.CategoryId;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnOrganizationCategoryPosted);

        //step 3
        // Need to check whether the route name exists
        if (await _onboardingWorkflow.GetRouteExistsAsync(organization.TenantRouteName))
        {
            ViewBag.TenantRouteExists = true;
            ViewBag.TenantNameEntered = organization.TenantRouteName;
            return BadRequest("Error occured while processing you request");
        }
        _onboardingWorkflow.OnboardingWorkflowItem.TenantRouteName = organization.TenantRouteName;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnTenantRouteNamePosted);

        //step 4
        _onboardingWorkflow.OnboardingWorkflowItem.ProductId = organization.ProductId;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnServicePlanPosted);

        //step 5
        await DeployTenantAsync();

        return LastAction(_onboardingWorkflow.OnboardingWorkflowItem.CategoryId);


        //return new JsonResult(new {message = "success"});
    }

    [HttpGet("/onboarding")]
    public IActionResult Tenantinfo()
    {
        return Ok(_onboardingWorkflow.OnboardingWorkflowItem);
    }


    /// <summary>
    /// Used by SPA application to check if suggested tenant name is available for use
    /// </summary>
    /// <param name="tn">Suggested tenant name</param>
    /// <returns>true or false depending on the availability</returns>
    [HttpGet("/Onboarding/tenants/name-avail")]
    public async Task<IActionResult> CheckTenantName(string tn)
    {
        bool isTenantNameAvailable = await _onboardingWorkflow.GetRouteExistsAsync(tn);

        return Ok(isTenantNameAvailable);
    }




   
    // Step 1 - Submit the organization name
    [HttpGet]
    public IActionResult OrganizationName()
    {
            ViewBag.OrganizationName = _onboardingWorkflow.OnboardingWorkflowItem.OrganizationName;
            return View();
    }

    // Step 1 - Submit the organization name
    [ValidateAntiForgeryToken]
    [HttpPost]
    public IActionResult OrganizationName(string organizationName)
    {
        _onboardingWorkflow.OnboardingWorkflowItem.OrganizationName = organizationName;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnOrganizationNamePosted);
        
        return RedirectToAction(SR.OrganizationCategoryAction, SR.OnboardingWorkflowController);
    }

    // Step 2 - Organization Category
    [Route(SR.OnboardingWorkflowOrganizationCategoryRoute)]
    [HttpGet]
    public IActionResult OrganizationCategory()
    {
        ViewBag.CategoryId = _onboardingWorkflow.OnboardingWorkflowItem.CategoryId; 
        return View(ReferenceData.TenantCategories);
    }

    // Step 2 Submitted - Organization Category
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult OrganizationCategoryAsync(int categoryId)
    {
        _onboardingWorkflow.OnboardingWorkflowItem.CategoryId = categoryId;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnOrganizationCategoryPosted);

        return RedirectToAction(SR.TenantRouteNameAction, SR.OnboardingWorkflowController);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult OrganizationCategoryBack(int categoryId)
    {
        return RedirectToAction(SR.OrganizationNameAction, SR.OnboardingWorkflowController);
    }

    // Step 3 - Tenant Route Name
    [HttpGet]
    public IActionResult TenantRouteName()
    {
        ViewBag.TenantRouteName = _onboardingWorkflow.OnboardingWorkflowItem.TenantRouteName; 
        return View();
    }

    // Step 3 Submitted - Tenant Route Name
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TenantRouteName(string tenantRouteName)
    {
        // Need to check whether the route name exists
        if (await _onboardingWorkflow.GetRouteExistsAsync(tenantRouteName))
        {
            ViewBag.TenantRouteExists = true;
            ViewBag.TenantNameEntered = tenantRouteName;
            return View();
        }

        _onboardingWorkflow.OnboardingWorkflowItem.TenantRouteName = tenantRouteName;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnTenantRouteNamePosted);

        return RedirectToAction(SR.ServicePlansAction, SR.OnboardingWorkflowController);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult TenantRouteNameBack(string tenantRouteName)
    {
        return RedirectToAction(SR.OrganizationCategoryAction, SR.OnboardingWorkflowController);
    }

    // Step 4 - Service Plan
    [HttpGet]
    public IActionResult ServicePlans()
    {
        return View();
    }

    // Step 4 Submitted - Service Plan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ServicePlans(int productId)
    {
        _onboardingWorkflow.OnboardingWorkflowItem.ProductId = productId;
        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnServicePlanPosted);

        return RedirectToAction(SR.ConfirmationAction, SR.OnboardingWorkflowController);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ServicePlansBack()
    {
        return RedirectToAction(SR.TenantRouteNameAction, SR.OnboardingWorkflowController);
    }

    // Step 5 - Tenant Created Confirmation
    [HttpGet]
    public async Task<IActionResult> Confirmation()
    {
        // Deploy the Tenant
        await DeployTenantAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LastAction(int categoryId)
    {
        var action = GetAction();
        return RedirectToAction(action, SR.OnboardingWorkflowController);
    }

    private async Task DeployTenantAsync()
    {
        await _onboardingWorkflow.OnboardTenant();

        UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnTenantDeploymentSuccessful);
    }

    private void UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers trigger)
    {
        _onboardingWorkflow.TransitionState(trigger);
        _onboardingWorkflow.PersistToSession();
    }

    private string GetAction()
    {
        var action = SR.OrganizationNameAction;

        if (!String.IsNullOrEmpty(_onboardingWorkflow.OnboardingWorkflowItem.TenantRouteName))
            action = SR.ServicePlansAction;
        else if (_onboardingWorkflow.OnboardingWorkflowItem.CategoryId > 0)
            action = SR.TenantRouteNameAction;

        return action;
    }

}


/// <summary>
/// A model for organization registration
/// Derived from the previous onboarding flow
/// </summary>
public class Organization
{
    public string OrganizationName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string TenantRouteName { get; set; } = string.Empty;

    public int ProductId { get; set; }


}

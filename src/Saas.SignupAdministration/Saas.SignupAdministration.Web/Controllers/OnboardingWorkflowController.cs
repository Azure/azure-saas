
using Saas.SignupAdministration.Web.Services.StateMachine;

namespace Saas.SignupAdministration.Web.Controllers;

[Authorize()]
// [AuthorizeForScopes(Scopes = new string[] { "tenant.read", "tenant.global.read", "tenant.write", "tenant.global.write", "tenant.delete", "tenant.global.delete" })]
public class OnboardingWorkflowController : Controller
{
    private readonly OnboardingWorkflowService _onboardingWorkflow;

    public OnboardingWorkflowController(OnboardingWorkflowService onboardingWorkflow)
    {
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
    public async Task<IActionResult> HandleBatchRegistration([FromBody] NewOnboardingItem organization)
    {
        if (!ModelState.IsValid) //Return a bad request
            return BadRequest("Cannot process your request");

        // Need to check whether the route name exists
        if (await _onboardingWorkflow.GetRouteExistsAsync(organization.TenantRouteName))
        {
            return BadRequest("Organization route name used is already taken");
        }

        _onboardingWorkflow.OnboardingWorkflowItem.OrganizationName = organization.OrganizationName;
        _onboardingWorkflow.OnboardingWorkflowItem.CategoryId = organization.CategoryId;
        _onboardingWorkflow.OnboardingWorkflowItem.TenantRouteName = organization.TenantRouteName;
        _onboardingWorkflow.OnboardingWorkflowItem.ProductId = organization.ProductTierId;
        _onboardingWorkflow.OnboardingWorkflowItem.Answer = organization.Answer;
        _onboardingWorkflow.OnboardingWorkflowItem.Question = organization.Question;
        _onboardingWorkflow.OnboardingWorkflowItem.Profession = organization.Profession;
        _onboardingWorkflow.OnboardingWorkflowItem.TimeZone = organization.TimeZone;
        _onboardingWorkflow.OnboardingWorkflowItem.NoofEmployees = organization.NoofEmployees;
        _onboardingWorkflow.OnboardingWorkflowItem.Country = organization.Country;

        await DeployTenantAsync();

        ///Change to created at action 
        return Ok(_onboardingWorkflow.OnboardingWorkflowItem.CategoryId);


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


    private async Task DeployTenantAsync()
    {
        await _onboardingWorkflow.OnboardTenant();

       // UpdateOnboardingSessionAndTransitionState(OnboardingWorkflowState.Triggers.OnTenantDeploymentSuccessful);
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
public class NewOnboardingItem
{
    public string OrganizationName { get; set; } = string.Empty;

    //Industry
    public int CategoryId { get; set; }

    public string TenantRouteName { get; set; } = string.Empty;

    public int ProductTierId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string Profession { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int NoofEmployees { get; set; } 
}

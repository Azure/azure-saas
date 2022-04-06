using Saas.SignupAdministration.Web.Services.StateMachine;

namespace Saas.SignupAdministration.Web.Controllers
{
    [Authorize()]
    public class OnboardingWorkflowController : Controller
    {
        private readonly ILogger<OnboardingWorkflowController> _logger;
        private readonly OnboardingWorkflow _onboardingWorkflow;

        public OnboardingWorkflowController(ILogger<OnboardingWorkflowController> logger, OnboardingWorkflow onboardingWorkflow)
        {
            _logger = logger;
            _onboardingWorkflow = onboardingWorkflow;
        }

        // Step 1 - Submit the organization name
        [HttpGet]
        public IActionResult OrganizationName()
        {
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
            // Populate Categories dropdown list
            List<Category> categories = new()
            {
                new Category { Id = 1, Name = SR.AutomotiveMobilityAndTransportationPrompt },
                new Category { Id = 2, Name = SR.EnergyAndSustainabilityPrompt },
                new Category { Id = 3, Name = SR.FinancialServicesPrompt },
                new Category { Id = 4, Name = SR.HealthcareAndLifeSciencesPrompt },
                new Category { Id = 5, Name = SR.ManufacturingAndSupplyChainPrompt },
                new Category { Id = 6, Name = SR.MediaAndCommunicationsPrompt },
                new Category { Id = 7, Name = SR.PublicSectorPrompt },
                new Category { Id = 8, Name = SR.RetailAndConsumerGoodsPrompt },
                new Category { Id = 9, Name = SR.SoftwarePrompt }
            };

            return View(categories);
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

        // Step 3 - Tenant Route Name
        [HttpGet]
        public IActionResult TenantRouteName()
        {

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

        // Step 5 - Tenant Created Confirmation
        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            // Deploy the Tenant
            await DeployTenantAsync();

            return View();
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
    }
}

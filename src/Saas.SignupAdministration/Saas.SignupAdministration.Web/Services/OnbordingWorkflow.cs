using Saas.SignupAdministration.Web.Services.StateMachine;


namespace Saas.SignupAdministration.Web.Services
{
    public class OnboardingWorkflow
    {
        private readonly IAdminServiceClient _adminServiceClient;
        private readonly IPersistenceProvider _persistenceProvider;
        private readonly IApplicationUser _applicationUser;
        private readonly IEmail _email;

        public OnboardingWorkflowItem OnboardingWorkflowItem { get; internal set; }
        public OnboardingWorkflowState OnboardingWorkflowState { get; internal set; }

        public OnboardingWorkflowState.States CurrentState
        {
            get
            {
                return OnboardingWorkflowState.CurrentState;

            }
        }

        public OnboardingWorkflow(IApplicationUser applicationUser, IAdminServiceClient adminServiceClient, IPersistenceProvider persistenceProvider, IEmail email)
        {
            _applicationUser = applicationUser;
            _adminServiceClient = adminServiceClient;
            _persistenceProvider = persistenceProvider;
            _email = email;

            OnboardingWorkflowItem item = _persistenceProvider.Retrieve<OnboardingWorkflowItem>(SR.OnboardingWorkflowItemKey);
            OnboardingWorkflowState state = _persistenceProvider.Retrieve<OnboardingWorkflowState>(SR.OnboardingWorkflowStateKey);

            OnboardingWorkflowItem = (item is null) ? new() : item;
            OnboardingWorkflowState = (state is null) ? new() : state;
        }

        public void TransitionState(OnboardingWorkflowState.Triggers trigger)
        {
            OnboardingWorkflowState.CurrentState = OnboardingWorkflowState.Transition(trigger);
        }

        public async Task OnboardTenant()
        {
            NewTenantRequest tenantRequest = new()
            {
                Name = OnboardingWorkflowItem.OrganizationName,
                Route = OnboardingWorkflowItem.TenantRouteName,
                CreatorEmail = _applicationUser.EmailAddress,
                ProductTierId = OnboardingWorkflowItem.ProductId,
                CategoryId = OnboardingWorkflowItem.CategoryId
            };

            // Call new Admin API
            await _adminServiceClient.TenantsPOSTAsync(tenantRequest);

            OnboardingWorkflowItem.IsComplete = true;
            OnboardingWorkflowItem.Created = DateTime.Now;
           
            _email.Send(_applicationUser.EmailAddress);
        }

        public void PersistToSession()
        {
            _persistenceProvider.Persist(SR.OnboardingWorkflowStateKey, OnboardingWorkflowState);
            _persistenceProvider.Persist(SR.OnboardingWorkflowItemKey, OnboardingWorkflowItem);
        }

        public async Task<bool> GetRouteExistsAsync(string route)
        {
            return !await _adminServiceClient.IsValidPathAsync(route);
        }
    }
}

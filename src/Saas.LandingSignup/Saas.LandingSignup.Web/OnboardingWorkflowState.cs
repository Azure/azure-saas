using Stateless;

namespace Saas.LandingSignup.Web
{
    public class OnboardingWorkflowState
    {
        public enum State { UserNameEntry, OrganizationNameEntry, OrganizationCategoryEntry, ServicePlanEntry, TenantDeploymentRequested, TenantDeploymentConfirmation, UsernameExistsError, Error };
        public enum Trigger { OnUserNamePosted, OnUserNameExists, OnOrganizationNamePosted, OnOrganizationCategoryPosted, OnServicePlanPosted, OnTenantDeploymentSuccessful, OnError, OnConfirmation };

        private readonly StateMachine<State, Trigger> stateMachine;

        public StateMachine<State, Trigger> StateMachine
        {
            get
            {
                return stateMachine;
            }
        }

        public OnboardingWorkflowState()
        {
            stateMachine = new StateMachine<State, Trigger>(State.UserNameEntry);

            stateMachine.Configure(State.UserNameEntry)
                .Permit(Trigger.OnUserNamePosted, State.OrganizationNameEntry)
                .Permit(Trigger.OnUserNameExists, State.UsernameExistsError)
                .Permit(Trigger.OnError, State.Error);

            stateMachine.Configure(State.OrganizationNameEntry)
                .Permit(Trigger.OnOrganizationNamePosted, State.OrganizationCategoryEntry)
                .Permit(Trigger.OnError, State.Error);

            stateMachine.Configure(State.OrganizationCategoryEntry)
                .Permit(Trigger.OnOrganizationCategoryPosted, State.ServicePlanEntry)
                .Permit(Trigger.OnError, State.Error);

            stateMachine.Configure(State.ServicePlanEntry)
                .Permit(Trigger.OnServicePlanPosted, State.TenantDeploymentRequested)
                .Permit(Trigger.OnError, State.Error);

            stateMachine.Configure(State.TenantDeploymentRequested)
                .Permit(Trigger.OnTenantDeploymentSuccessful, State.TenantDeploymentConfirmation)
                .Permit(Trigger.OnError, State.Error);
        }
    }
}

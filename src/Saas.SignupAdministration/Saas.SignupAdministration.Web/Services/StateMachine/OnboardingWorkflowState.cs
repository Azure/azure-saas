using Newtonsoft.Json;

namespace Saas.SignupAdministration.Web.Services.StateMachine;

public class OnboardingWorkflowState
{
    public enum States
    {
        OrganizationNameEntry,
        OrganizationCategoryEntry,
        TenantRouteNameEntry,
        ServicePlanEntry,
        TenantDeploymentRequested,
        TenantDeploymentConfirmation,
        Error
    };

    public enum Triggers
    {
        OnOrganizationNamePosted,
        OnOrganizationCategoryPosted,
        OnTenantRouteNamePosted,
        OnServicePlanPosted,
        OnTenantDeploymentSuccessful,
        OnError,
        OnOrganizationCategoryBack,
        OnTenantRouteNameBack,
        OnServicePlanBack, 
        OnJumpToLast
    };

    [JsonProperty(PropertyName = SR.OnboardingWorkflowStateCurrentStateProperty)]
    public States CurrentState { get; internal set; }

    public OnboardingWorkflowState(States state = States.OrganizationNameEntry)
    {
        CurrentState = state;
    }

    public States Transition(Triggers trigger)
    {
        ChangeState(CurrentState, trigger);
        return CurrentState;
    }

    private States ChangeState(States current, Triggers trigger) =>
        CurrentState = ((current, trigger) switch
        {
            // Step 1 - Submit the organization name
            (States.OrganizationNameEntry, Triggers.OnOrganizationNamePosted) => States.OrganizationCategoryEntry,
            (States.OrganizationCategoryEntry, Triggers.OnOrganizationNamePosted) => States.OrganizationCategoryEntry,
            (States.TenantRouteNameEntry, Triggers.OnOrganizationNamePosted) => States.OrganizationCategoryEntry,
            (States.ServicePlanEntry, Triggers.OnOrganizationNamePosted) => States.OrganizationCategoryEntry,
            (States.OrganizationNameEntry, Triggers.OnError) => States.Error,
            // Step 2 - Organization Category
            (States.OrganizationCategoryEntry, Triggers.OnOrganizationCategoryPosted) => States.TenantRouteNameEntry,
            (States.OrganizationNameEntry, Triggers.OnOrganizationCategoryPosted) => States.TenantRouteNameEntry,
            (States.TenantRouteNameEntry, Triggers.OnOrganizationCategoryPosted) => States.TenantRouteNameEntry,
            (States.ServicePlanEntry, Triggers.OnOrganizationCategoryPosted) => States.TenantRouteNameEntry,
            (States.OrganizationCategoryEntry, Triggers.OnError) => States.Error,
            // Step 3 - Tenant Route Name
            (States.TenantRouteNameEntry, Triggers.OnTenantRouteNamePosted) => States.ServicePlanEntry,
            (States.OrganizationNameEntry, Triggers.OnTenantRouteNamePosted) => States.ServicePlanEntry,
            (States.OrganizationCategoryEntry, Triggers.OnTenantRouteNamePosted) => States.ServicePlanEntry,
            (States.ServicePlanEntry, Triggers.OnTenantRouteNamePosted) => States.ServicePlanEntry,
            (States.TenantRouteNameEntry, Triggers.OnError) => States.Error,
            // Step 4 - Service Plan
            (States.ServicePlanEntry, Triggers.OnServicePlanPosted) => States.TenantDeploymentRequested,
            (States.OrganizationNameEntry, Triggers.OnServicePlanPosted) => States.TenantDeploymentRequested,
            (States.OrganizationCategoryEntry, Triggers.OnServicePlanPosted) => States.TenantDeploymentRequested,
            (States.TenantRouteNameEntry, Triggers.OnServicePlanPosted) => States.TenantDeploymentRequested,
            (States.ServicePlanEntry, Triggers.OnError) => States.Error,
            // Step 5 - Tenant Created Confirmation
            (States.TenantDeploymentRequested, Triggers.OnTenantDeploymentSuccessful) => States.TenantDeploymentConfirmation,
            (States.TenantDeploymentRequested, Triggers.OnError) => States.Error,

            _ => throw new NotSupportedException($"{current} has no transition on {trigger}")
        });

}

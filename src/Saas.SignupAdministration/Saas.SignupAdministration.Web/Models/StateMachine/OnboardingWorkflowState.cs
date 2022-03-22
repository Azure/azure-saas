using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Saas.SignupAdministration.Web.Models.StateMachine
{
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
        };

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

        States ChangeState(States current, Triggers trigger) =>
            CurrentState = ((current, trigger) switch
            {
                (States.OrganizationNameEntry, Triggers.OnOrganizationNamePosted) => States.OrganizationCategoryEntry,
                (States.OrganizationNameEntry, Triggers.OnError) => States.Error,
                (States.OrganizationCategoryEntry, Triggers.OnOrganizationCategoryPosted) => States.TenantRouteNameEntry,
                (States.OrganizationCategoryEntry, Triggers.OnError) => States.Error,
                (States.TenantRouteNameEntry, Triggers.OnTenantRouteNamePosted) => States.ServicePlanEntry,
                (States.TenantRouteNameEntry, Triggers.OnError) => States.Error,
                (States.ServicePlanEntry, Triggers.OnServicePlanPosted) => States.TenantDeploymentRequested,
                (States.ServicePlanEntry, Triggers.OnError) => States.Error,
                (States.TenantDeploymentRequested, Triggers.OnTenantDeploymentSuccessful) => States.TenantDeploymentConfirmation,
                (States.TenantDeploymentRequested, Triggers.OnError) => States.Error,
                _ => throw new NotSupportedException($"{current} has no transition on {trigger}")
            });
    }
}

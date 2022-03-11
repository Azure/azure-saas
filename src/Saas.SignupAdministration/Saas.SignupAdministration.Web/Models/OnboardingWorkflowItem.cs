using Newtonsoft.Json;
using Saas.SignupAdministration.Web.Models.StateMachine;
using System;

namespace Saas.SignupAdministration.Web.Models
{
    public class OnboardingWorkflowItem
    {
        [JsonProperty(PropertyName = SR.OnboardingWorkflowEmailAddressProperty)]
        public string EmailAddress { get; set; }   

        [JsonProperty(PropertyName = SR.OnboardingWorkflowIdProperty)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowNameProperty)]
        public string OnboardingWorkflowName { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowTenantNameProperty)]
        public string TenantName { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowUserIdProperty)]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowIsExistingUserProperty)]
        public string IsExistingUser { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowCategoryIdProperty)]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowProductIdProperty)]
        public int ProductId { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowIsCompleteProperty)]
        public bool IsComplete { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowIpAddressProperty)]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowCreatedProperty)]
        public DateTime Created { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowOrganizationNameProperty)]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = SR.OnboardingWorkflowStateProperty)]
        public OnboardingWorkflowState.States CurrentWorkflowState { get; set; }
    }
}

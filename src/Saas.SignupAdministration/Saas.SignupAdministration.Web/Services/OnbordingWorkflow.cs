using Saas.SignupAdministration.Web.Models;
using System;
using System.Net.Http;
using Saas.SignupAdministration.Web.Services.StateMachine;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;

namespace Saas.SignupAdministration.Web.Services
{
    public class OnboardingWorkflow
    {
        private AppSettings _appSettings;

        public OnboardingWorkflowItem OnboardingWorkflowItem { get; internal set; }
        public OnboardingWorkflowState OnboardingWorkflowState { get; internal set; }

        public OnboardingWorkflowState.States CurrentState
        {
            get
            {
                return OnboardingWorkflowState.CurrentState;

            }
        }

        public OnboardingWorkflow(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            var session = AppHttpContext.Current.Session;

            OnboardingWorkflowItem item = session.GetObjectFromJson<OnboardingWorkflowItem>(SR.OnboardingWorkflowItemKey);
            OnboardingWorkflowState state = session.GetObjectFromJson<OnboardingWorkflowState>(SR.OnboardingWorkflowStateKey);

            OnboardingWorkflowItem = (item == null) ? new() : item;
            OnboardingWorkflowState = (state == null) ? new() : state;
        }

        public void TransitionState(OnboardingWorkflowState.Triggers trigger)
        {
            OnboardingWorkflowState.CurrentState = OnboardingWorkflowState.Transition(trigger);
        }

        public async Task OnboardTenet()
        {
            HttpClient httpClient = new();
            OnboardingClient onboardingClient = new OnboardingClient(_appSettings.OnboardingApiBaseUrl, httpClient);

            Tenant tenant = new()
            {
                Id = Guid.NewGuid(),
                Name = OnboardingWorkflowItem.Id,
                IsActive = true,
                IsCancelled = false,
                IsProvisioned = true,
                ApiKey = Guid.NewGuid(),
                CategoryId = OnboardingWorkflowItem.CategoryId,
                ProductId = OnboardingWorkflowItem.ProductId,
                UserId = OnboardingWorkflowItem.UserId
            };

            //TODO: Call new Admin API
            //await onboardingClient.TenantsPOSTAsync(tenant);

            OnboardingWorkflowItem.IsComplete = true;
            OnboardingWorkflowItem.Created = DateTime.Now;
        }

        public void PersistToSession()
        {
            var session = AppHttpContext.Current.Session;

            session.SetObjectAsJson(SR.OnboardingWorkflowStateKey, OnboardingWorkflowState);
            session.SetObjectAsJson(SR.OnboardingWorkflowItemKey, OnboardingWorkflowItem);
        }
    }
}

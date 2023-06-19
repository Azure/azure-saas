using Newtonsoft.Json;
using Saas.SignupAdministration.Web.Services.StateMachine;

namespace Saas.SignupAdministration.Web.Services;

public class OnboardingWorkflowItem
{
    [JsonProperty(PropertyName = SR.OnboardingWorkflowIdProperty)]
    public Guid Id { get; set; }

    [JsonIgnore]
    [JsonProperty(PropertyName = SR.OnboardingWorkflowNameProperty)]
    public string OnboardingWorkflowName { get; set; } = string.Empty;

    [JsonProperty(PropertyName = SR.OnboardingWorkflowUserIdProperty)]
    public string UserId { get; set; } = string.Empty;

    [JsonProperty(PropertyName = SR.OnboardingWorkflowCategoryIdProperty)]
    public int CategoryId { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowProductIdProperty)]
    public int ProductId { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowIsCompleteProperty)]
    public bool IsComplete { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowCreatedProperty)]
    public DateTime Created { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowOrganizationNameProperty)]
    public string OrganizationName { get; set; } = string.Empty;

    [JsonProperty(PropertyName = SR.OnboardingWorkflowStateProperty)]
    public OnboardingWorkflowState.States CurrentWorkflowState { get; set; }


    /// <summary>
    /// Start of custom defined fields
    /// </summary>
    public string Question { get; set; } = string.Empty;

    [JsonIgnore]
    public string Answer { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string Profession { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int NoofEmployees { get; set; }
    /// <summary>
    /// End of custom defined fields
    /// </summary>

    [JsonProperty(PropertyName = SR.OnboardingWorkflowIsActiveProperty)]
    public bool IsActive { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowIsCancelledProperty)]
    public bool IsCancelled { get; set; }

    [JsonProperty(PropertyName = SR.OnboardingWorkflowIsProvisionedProperty)]
    public bool IsProvisioned { get; set; }

    public OnboardingWorkflowItem()
    {
        Initialize();
    }

    public OnboardingWorkflowItem(string userId)
    {
        Id = Guid.NewGuid();
        OnboardingWorkflowName = SR.OnboardingWorkflowName;
        UserId = userId;
        Created = DateTime.Now;
    }

    private void Initialize()
    {
        Id = Guid.NewGuid();
        OnboardingWorkflowName = SR.OnboardingWorkflowName;
        UserId = Guid.NewGuid().ToString();
        Created = DateTime.Now;
    }
}

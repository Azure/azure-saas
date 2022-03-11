using System;

namespace Saas.SignupAdministration.Web
{
    internal static class SR
    {
        // Internal Data Names
        public const string OnboardingWorkflowName = "Onboarding Workflow";

        // Prompts
        public const string AutomotiveMobilityAndTransportationPrompt = "Automotive, Mobility & Transportation";
        public const string EnergyAndSustainabilityPrompt = "Energy & Sustainability";
        public const string FinancialServicesPrompt = "Financial Services";
        public const string HealthcareAndLifeSciencesPrompt = "Healthcare & Life Sciences";
        public const string ManufacturingAndSupplyChainPrompt = "Manufacturing & Supply Chain";
        public const string MediaAndCommunicationsPrompt = "Media & Communications";
        public const string PublicSectorPrompt = "Public Sector";
        public const string RetailAndConsumerGoodsPrompt = "Retail & Consumer Goods";
        public const string SoftwarePrompt = "Software";
        public const string EmailPrompt = "Email";
        public const string PasswordPrompt = "Password";

        // API Route Template
        public const string ApiRouteTemplate = "api/[controller]";

        // Tenant Variable Template
        public const string TenantTemplate = "{tenant}";

        // Password Error Message Template
        public const string PasswordErrorMessageTemplate = "The {0} must be at least {2} and at max {1} characters long.";

        // Controller Names
        public const string OnboardingWorkflowController = "OnboardingWorkflow";

        // Controller Actions
        public const string OrganizationNameAction = "OrganizationName";
        public const string OrganizationCategoryAction = "OrganizationCategory";
        public const string ServicePlansAction = "ServicePlans";
        public const string DeployTenantAction = "DeployTenant";
        public const string ConfirmationAction = "Confirmation";
        public const string MerchantAction = "Merchant";
        public const string IndexAction = "Index";
        public const string UsernameAction = "Username";
        public const string TenantRouteNameAction = "TenantRouteName";

        // Controller Routes
        public const string OnboardingWorkflowDeployRoute = "/" + OnboardingWorkflowController + "/" + DeployTenantAction;
        public const string OnboardingWorkflowConfirmationRoute = "/" + OnboardingWorkflowController + "/" + ConfirmationAction;
        public const string OnboardingWorkflowOrganizationNameRoute = "/" + OnboardingWorkflowController + "/" + OrganizationNameAction;
        public const string OnboardingWorkflowOrganizationCategoryRoute = "/" + OnboardingWorkflowController + "/" + OrganizationCategoryAction;
        public const string OnboardingWorkflowServicePlansRoute = "/" + OnboardingWorkflowController + "/" + ServicePlansAction;
        public const string OnboardingWorkflowUsernameRoute = "/" + OnboardingWorkflowController + "/" + UsernameAction;
        public const string TenantRoute = "/" + SR.TenantTemplate;

        // Session Variables
        public const string TenantId = "TenantId";

        // Header Variables
        public const string XApiKey = "X-Api-Key";

        // Onboarding Workflow Properties
        public const string OnboardingWorkflowIdProperty = "id";
        public const string OnboardingWorkflowNameProperty = "onboardingWorkflowName";
        public const string OnboardingWorkflowTenantNameProperty = "tenantName";
        public const string OnboardingWorkflowUserIdProperty = "userId";
        public const string OnboardingWorkflowIsExistingUserProperty = "isExistingUser";
        public const string OnboardingWorkflowCategoryIdProperty = "categoryId";
        public const string OnboardingWorkflowProductIdProperty = "productId";
        public const string OnboardingWorkflowIsCompleteProperty = "isComplete";
        public const string OnboardingWorkflowIpAddressProperty = "ipAddress";
        public const string OnboardingWorkflowCreatedProperty = "created";
        public const string OnboardingWorkflowStateProperty = "state";
        public const string OnboardingWorkflowEmailAddressProperty = "emailAddress";
        public const string OnboardingWorkflowOrganizationNameProperty = "organizationName";
        public const string OnboardingWorkflowTenantRouteNameProperty = "tenantRouteName";

        // Temp Data KEys
        public const string OnboardingWorkflowItemKey = "OnboardingWorkflowItem";

        // AppSettings Properties
        public const string CatalogDbConnectionProperty = "ConnectionStrings:CatalogDbConnection";

        // Catalog DB Prperties
        public const string CatalogTenantIdProperty = "TenantId";
        public const string CatalogTenantIdParameter = "@" + CatalogTenantIdProperty;
        public const string CatalogIdProperty = "Id";
        public const string CatalogCustomerNameProperty = "CustomerName";
        public const string CatalogIsActiveProperty = "IsActive";
        public const string CatalogApiKeyParameter = "@apiKey";
        public const string CatalogCustomerSelectQuery = "SELECT * FROM dbo.Customer Where TenantId = " + CatalogTenantIdParameter;
        public const string CatalogTenantSelectQuery = "SELECT Id  FROM Tenant WHERE ApiKey = " + CatalogApiKeyParameter;

        // Azure AD Properties
        public const string AzureAdAuthorityFormat = "https://login.microsoftonline.com/{0}/v2.0";

        // Startup Properties
        public const string IdentityDbConnectionProperty = "IdentityDbConnection";
        public const string AppSettingsProperty = "AppSettings";
        public const string AppInsightsConnectionProperty = "APPINSIGHTS_CONNECTIONSTRING";
        public const string CosmosDbProperty = "AppSettings:CosmosDb";
        public const string ErrorRoute = "/Home/Error";
        public const string MapControllerRoutePattern = "{controller=Home}/{action=Index}/{ id ?}";
        public const string DefaultName = "default";
        public const string DatabaseNameProperty = "DatabaseName";
        public const string ContainerNameProperty = "ContainerName";
        public const string AccountProperty = "Account";
        public const string KeyProperty = "Key";
        public const string CosmosNamePartitionKey = "/name";

        // Error Codes
        public const string DuplicateUserNameErrorCode = "DuplicateUserName";
    }
}

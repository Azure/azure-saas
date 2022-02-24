using System;

namespace Saas.LandingSignup.Web
{
    internal static class SR
    {
        // Internal Data Names
        public const string OnboardingFlowName = "Onboarding Flow";

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
        public const string CreateController = "Create";

        // Controller Actions
        public const string NameAction = "Name";
        public const string CategoryAction = "Category";
        public const string PlansAction = "Plans";
        public const string DeployAction = "Deploy";
        public const string ConfirmationAction = "Confirmation";
        public const string MerchantAction = "Merchant";

        // Session Variables
        public const string TenantId = "TenantId";

        // Header Variables
        public const string XApiKey = "X-Api-Key";

        // Cosmos DB Properties
        public const string CosmosIdProperty = "id";
        public const string CosmosNameProperty = "name";
        public const string CosmosTenantNameProperty = "tenantName";
        public const string CosmosUserIdProperty = "userId";
        public const string CosmosIsExistingUserProperty = "isExistingUser";
        public const string CosmosCategoryIdProperty = "categoryId";
        public const string CosmosProductIdProperty = "productId";
        public const string CosmosIsCompleteProperty = "isComplete";
        public const string CosmosIpAddressProperty = "ipAddress";
        public const string CosmosCreatedProperty = "created";

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
    }
}

namespace Saas.Application.Web;

public static class SR
{
    // Catalog DB Prperties
    public const string CatalogTenantIdProperty = "TenantId";
    public const string CatalogTenantIdParameter = "@" + CatalogTenantIdProperty;
    public const string CatalogApiKeyParameter = "@apiKey";

    // Startup Properties
    public const string AppSettingsProperty = "AppSettings";
    public const string AppInsightsConnectionProperty = "APPINSIGHTS_CONNECTIONSTRING";
    public const string ErrorRoute = "/Home/Error";
    public const string MapControllerRoutePattern = "{controller=Home}/{action=Index}/{ id ?}";
    public const string DefaultName = "default";
    public const string AdminServiceBaseUrl = "AppSettings:AdminServiceBaseUrl";
    public const string AdminServiceScopesProperty = "AppSettings:AdminServiceScopes";
    public const string AzureAdB2CProperty = "AzureAdB2C";
    public const string KeyVaultProperty = "KeyVault:Url";

    // Claim Types
    public const string EmailAddressClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public const string NameIdentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string AuthenticationClassReferenceClaimType = "http://schemas.microsoft.com/claims/authnclassreference";
    public const string AuthenticationTimeClaimType = "auth_time";
    public const string GivenNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    public const string SurnameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
}

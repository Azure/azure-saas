namespace Saas.Application.Web;

public static class SR
{
    //Project name used for scoped css reference despite renaming
    public static readonly string ProjectName = typeof(SR).Assembly.GetName().Name ?? string.Empty;

    //Tenant Page

    // Catalog DB Prperties
    public const string CatalogTenantIdProperty = "TenantId";
    public const string CatalogTenantIdParameter = "@" + CatalogTenantIdProperty;
    public const string CatalogApiKeyParameter = "@apiKey";

    // Startup Properties
    public const string ErrorRoute = "/Error";
    public const string MapControllerRoutePattern = "{action=Index}/{ id ?}";
    public const string DefaultName = "default";

    // Claim Types
    public const string EmailAddressClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public const string NameIdentifierClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string AuthenticationClassReferenceClaimType = "http://schemas.microsoft.com/claims/authnclassreference";
    public const string AuthenticationTimeClaimType = "auth_time";
    public const string GivenNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    public const string SurnameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
}

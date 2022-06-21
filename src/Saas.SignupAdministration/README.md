# Saas.SignupAdministration.Web

## 1. Module Overview

This project hosts an application for the onboarding and administration of new tenants in your SaaS ecosystem. It is fully self-contained such that it includes complete copies of all necessary classes for operation. However, keep in mind that some functionality within the app does have [dependencies](https://azure.github.io/azure-saas/components/signup-administration#dependencies) on other services

For a complete overview, please see the [SaaS.SignupAdministration.Web](https://azure.github.io/azure-saas/components/signup-administration/) page in our documentation site.

The application has been developed in [MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-6.0) format, its pages built by respective Controllers paired to Views. See the Views and Controllers directories for relevant service logic or display logic.

## 2. How to Run Locally

Once configured, this app creates new Tenants to be persisted by the Admin service. These tenants can then be administrated and have users added to them for reference from the proper SaaS application. System users in this process do not require any preconfigured roles to use the new tenant workflow.

### i. Requirements

To run the web api, you must have the following installed on your machine:

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [ASP.NET Core 6.0](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
- (Reccomended) [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)
- A user store compatible with Microsoft Identity
    - [Azure AD B2C](https://azure.microsoft.com/en-us/services/active-directory/external-identities/b2c/) - created automatically with Bicep deployment

### ii. Development Tools

- [NSwag](https://github.com/RicoSuter/NSwag) - An NSwag configuration file has been included to generate an appropriate client from the included Admin project.
    *Consumes Clients:*
	- [admin-service-client-generator.nswag](Saas.SignupAdministration.Web/admin-service-client-generator.nswag)
	
### iii. App Settings

In order to run the project locally, the App Settings marked as `secret: true` must be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). If developing in Visual Studio, right click the project in the Solution Explorer and select `Manage User Secrets`.

> The secrets indicated here are for Azure AD B2C integration. You must have an existing user store to reference which is configured to redirect to localhost on your debugging port in order to function correctly.

When deployed to Azure using the Bicep deployments, these secrets are [loaded from Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment) instead.
Default values for non secret app settings can be found in [appsettings.json](Saas.SignupAdministration.Web/appsettings.json)

| AppSetting Key                              | Description                                                                    | Secret | Default Value                  |
| ------------------------------------------- | ------------------------------------------------------------------------------ | ------ | ------------------------------ |
| AllowedHosts                                | Allowed app host names, semicolon delimited, asterisk is wildcard              | false  | *                              |
| AppSettings:AdminServiceBaseUrl             | URL for downstream admin service                                               | false  | https://localhost:7041/        |
| AppSettings:AdminServiceScopeBaseUrl        | The B2C URL for the admin service scope                                        | false  |                                |
| AppSettings:AdminServiceScopes              | List of scopes to authorize user for on the admin service. Space delimited     | false  |                                |
| AzureAdB2C:ClientId                         | The service client corresponding to the Signup Admin application               | true   |                                |
| AzureAdB2C:ClientSecret                     | Unique secret for the application client provided to authenticate the app      | true   |                                |
| AzureAdB2C:Domain                           | Domain name for the Azure AD B2C instance                                      | true   |                                |
| AzureAdB2C:Instance                         | URL for the root of the Azure AD B2C instance                                  | true   |                                |
| AzureAdB2C:SignedOutCallbackPath            | Callback path (not full url) contacted after signout                           | false  | /signout/B2C_1A_SIGNUP_SIGNIN  |
| AzureAdB2C:SignUpSignInPolicyId             | Name of signup/signin policy                                                   | false  | B2C_1A_SIGNUP_SIGNIN           |
| AzureAdB2C:TenantId                         | Identifier for the overall Azure AD B2C tenant for the overall SaaS ecosystem  | true   |                                |
| EmailOptions:Body                           | Signup notification email body text                                            | false  |                                |
| EmailOptions:EndPoint                       | Service endpoint to send confirmation email                                    | true   |                                |
| EmailOptions:FromAddress                    | Signup notification email source                                               | false  |                                |
| EmailOptions:Subject                        | Signup notification email subject line                                         | false  |                                |
| KeyVault:Url                                | KeyVault URL to pull secret values from in production                          | false  |                                |
| Logging:LogLevel:Default                    | Logging level when no configured provider is matched                           | false  | Information                    |
| Logging:LogLevel:Microsoft                  | Logging level for Microsoft logging                                            | false  | Warning                        |
| Logging:LogLevel:Microsoft.Hosting.Lifetime | Logging level for Hosting Lifetime logging                                     | false  | Information                    |
 
### iv. Starting the App

These instructions cover running the app using its existing implementations and included modules locally. Substituting other module implementations or class implementations may make existing secrets irrelevant.

1. Insert secrets marked as required for running locally into your secrets manager (such as by using provided script).
1. Configure multiple projects to launch: Saas.SignupAdministration.Web, Saas.Permissions.Service and Saas.Admin.Service. In Visual Studio, this can be accomplished by right clicking the project and selecting `Set Startup Projects...` for local debugging.
1. Start apps. Services will launch as presented Swagger APIs. Web app will launch MVC ASP.NET Core application.

## 3. Additional Resources

### i. Azure AD B2C

You'll need to configure your B2C instance for an external authentication provider. Additional documentation is available [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/identity-provider-azure-ad-multi-tenant?pivots=b2c-user-flow).

### ii. JsonSessionPersistenceProvider

The JsonSessionPersistenceProvider maintains the state of the onboarding workflow for each user and allows for forward and backward movment in the app with access to all of the values of the Tenant. Custom providers can be used as long as they inherit from the IPersistenceProvider interface. 

The 2 methods are: 
- public void Persist(string key, object value);
- public T Retrieve<T>(string key);

The included implementation simply stores the session data within memory of the server app, making it highly unsuitable at scale or when multiple app instances are deployed. A caching service able to perform realtime updates is ideal to maximize scalability. Consider session management through industry standard tools such as [Redis](https://redis.com/solutions/use-cases/session-management/).
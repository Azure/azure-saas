# Saas.Application.Web

This project is a sample tenant-aware application. *This is where you'll put your own code*.

This sample default application is **Contoso BadgeMeUp**. It is easy to run the application locally to explore how it works within the context of the other SaaS modules.

![BadgeMeUp Screenshot](badgemeup-screenshot.gif)

Contoso BadgeMeUp is a simple SaaS B2B application that Contoso sells to companies that want a great tool to improve the culture within their organization.

## 1. Module Overview

This project hosts an application for providing the intended SaaS functions to valid Tenants. It is fully self-contained such that it includes complete copies of all necessary classes for operation. However, keep in mind that some functionality within the app does have [dependencies](https://azure.github.io/azure-saas/components/saas-application/#dependencies) on other services.

For a design overview, please see the [SaaS.Application.Web](https://azure.github.io/azure-saas/components/saas-application/) page in our documentation site.

The application has been developed using [Razor](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-6.0&tabs=visual-studio) pages with code behind. Automatic reference to the project name has been established through the string resource class (SR.cs) for the scoped CSS reference in _Layout.cshtml. See the Pages and Service directories for relevant display or service logic.

### i. Customizing the Template

Throughout the project, various "TODO (SaaS)" tags have been added. They are placed around identifiers you will want to alter to suit your individual application if you plan to build off of the template. Either navigate using the todo list in Visual Studio or search for "TODO (SaaS)" to quickly address these areas and find points to begin development.

### ii. Addressing Tenants

Basic authentication has been implemented utilizing Azure AD B2C and utilizing the same underlying claims as the Signup application. Further, a demonstration of tenant-specific app logic has been implemented as a starting guide to work by. When an authenticated user is signed into the application, you may visit the route path from the site base to fetch public information regarding the tenant and have it displayed. For instance, when you register the route "MySoftwareCompany" during signup and run the app locally, this will look like:

>https://localhost:5000/MySoftwareCompany

## 2. How to Run Locally

Once configured, this app creates new Tenants to be persisted by the Admin service. These tenants can then be administrated and have users added to them for reference from the proper SaaS application. System users in this process do not require any preconfigured roles to use the new tenant workflow.

### i. Requirements

To run the web api, you must have the following installed on your machine:

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [ASP.NET Core 6.0](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
- (Reccomended) [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)
- A deployed [Identity Framework](https://azure.github.io/azure-saas/quick-start/) instance
    - [Azure AD B2C](https://azure.microsoft.com/en-us/services/active-directory/external-identities/b2c/) - created automatically with Bicep deployment

### ii. Development Tools

- [NSwag](https://github.com/RicoSuter/NSwag) - An NSwag configuration file has been included to generate an appropriate client from the included Admin project.
    *Consumes Clients:*
	- [admin-service-client-generator.nswag](Saas.Application.Web/admin-service-client-generator.nswag)
	
### iii. App Settings

In order to run the project locally, the App Settings marked as `secret: true` must be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). If developing in Visual Studio, right click the project in the Solution Explorer and select `Manage User Secrets`.

> The secrets indicated here are for Azure AD B2C integration. You must have an existing user store to reference which is configured to redirect to localhost on your debugging port in order to function correctly.

When deployed to Azure using the Bicep deployments, these secrets are [loaded from Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment) instead.
Default values for non secret app settings can be found in [appsettings.json](Saas.Application.Web/appsettings.json)

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
| KeyVault:Url                                | KeyVault URL to pull secret values from in production                          | false  |                                |
| Logging:LogLevel:Default                    | Logging level when no configured provider is matched                           | false  | Information                    |
| Logging:LogLevel:Microsoft                  | Logging level for Microsoft logging                                            | false  | Warning                        |
| Logging:LogLevel:Microsoft.Hosting.Lifetime | Logging level for Hosting Lifetime logging                                     | false  | Information                    |
 
### iv. Starting the App

These instructions cover running the app using its existing implementations and included modules locally. Substituting other module implementations or class implementations may make existing secrets irrelevant.

1. Insert secrets marked as required for running locally into your secrets manager (such as by using provided script).
1. Configure multiple projects to launch: Saas.Application.Web, Saas.Permissions.Service and Saas.Admin.Service. In Visual Studio, this can be accomplished by right clicking the project and selecting `Set Startup Projects...` for local debugging.
1. Start apps. Services will launch as presented Swagger APIs. Web app will launch Razor page ASP.NET Core application.
1. Navigate from the welcome screen to a sub path matching one of your configured Tenant routes

## 3. Additional Resources

### i. Azure AD B2C
You'll need to configure your B2C instance for an external authentication provider. Additional documentation is available [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/identity-provider-azure-ad-multi-tenant?pivots=b2c-user-flow).
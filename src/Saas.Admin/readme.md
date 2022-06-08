# Saas.Admin.Service

The SaaS Admin Service is an API that is reponsible for tenant management operations. Within this folder, you will find 3 sections:

1. Saas.Admin.Service - The .NET Web API project containing the code for the API

2. Saas.Admin.Service.Deployment - The bicep module for deploying the infrastructure required to host the API in Azure

3. Saas.Admin.Service.Tests - Unit tests for the service

## 1. Module Overview

This project hosts a service api which serves as a gateway to administrate the SaaS ecosystem of Tenants. It is fully self-contained such that it includes complete copies of all necessary classes for operation. Since it contains no direct references to the other projects, it can be extracted to launch in isolation insofar as expected substitutes are configured.

For a complete overview, please see the [SaaS.Admin.Service](https://azure.github.io/azure-saas/components/admin-service/) page in our documentation site.

## 2. How to Run Locally

Once configured, this app presents an api service which manages authorization of actions issued to the Admin service. It may be run locally during development of service logic and for regenerating its included NSwag api client. (An NSwag file is included in the Admin project to generate its client.)

### i. Requirements

To run the web api, you must have the following installed on your machine:

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [ASP.NET Core 6.0](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
- (Reccomended) [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)
- A connection string to a running, empty SQL Server Database.
    - [Local DB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15) (Windows Only) - See `Additional Resources` below for basic config secret
    - [SQL Server Docker Container](https://hub.docker.com/_/microsoft-mssql-server)
    - [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- A user store compatible with Microsoft Identity
    - [Azure AD B2C](https://azure.microsoft.com/en-us/services/active-directory/external-identities/b2c/) - created automatically with Bicep deployment

### ii. Development Tools

- [NSwag](https://github.com/RicoSuter/NSwag) - An NSwag configuration file has been included to generate an appropriate client from the included Admin project.
    *Consumes Clients:*
	- [permissions-service-client-generator.nswag](Saas.Admin.Service/permissions-service-client-generator.nswag)
	*Consumed By:*
    - [Saas.SignupAdministration](../Saas.SignupAdministration)
    - [Saas.Application](../Saas.Application)

### iii. App Settings

In order to run the project locally, the App Settings marked as `secret: true` must be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). When deployed to azure using the bicep deployments, these secrets are [loaded from Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment) instead.

Default values for non secret app settings can be found in [appsettings.json](Saas.Admin.Service/appsettings.json)

| AppSetting Key                             |  Description                                                                                                                            | Secret | Default Value                         |
| ------------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------- | ------ | ------------------------------------- |
| AllowedHosts                               | Allowed app host names, semicolon delimited                                                                                             | false  | *                                     |
| AzureAdB2C:ClientId                        | The service client corresponding to the Signup Admin application                                                                        | true   |                                       |
| AzureAdB2C:Domain                          | Domain name for the Azure AD B2C instance                                                                                               | true   |                                       |
| AzureAdB2C:Instance                        | URL for the root of the Azure AD B2C instance                                                                                           | true   |                                       |
| AzureAdB2C:SignedOutCallbackPath           | Callback path (not full url) contacted after signout                                                                                    | false  | /signout/B2C_1A_SIGNUP_SIGNIN         |
| AzureAdB2C:SignUpSignInPolicyId            | Name of signup/signin policy                                                                                                            | false  | B2C_1A_SIGNUP_SIGNIN                  |
| AzureAdB2C:TenantId                        | Identifier for the overall Azure AD B2C tenant for the overall SaaS ecosystem                                                           | true   |                                       |
| ClaimToRoleTransformer:AuthenticationType  | Indicates the Authentication type for new identity                                                                                      | false  | MyCustomRoleAuth                      |
| ClaimToRoleTransformer:RoleClaimtype       | Type of the claim to use in the new Identity, works alongside built-in                                                                  | false  | MyCustomRoles                         |
| ClaimToRoleTransformer:SourceClaimType     | Name of the claim custom roles are in                                                                                                   | false  | permissions                           |
| ConnectionStrings:TenantsContext           | Connection String to SQL server database used to store permission data.                                                                 | true   |                                       |
| KeyVault:PermissionsApiCert                | The name of the secret in Azure Key Vault that contains a base64 encoded certificate to use for authentication with the permissions api | false  |                                       |
| KeyVault:Url                               | KeyVault URL to pull secret values from in production                                                                                   | false  |                                       |
| Logging:LogLevel:Default                   | Logging level when no configured provider is matched                                                                                    | false  | Information                           |
| Logging:LogLevel:Microsoft.AspNetCore      | Logging level for AspNetCore logging                                                                                                    | false  | Warning                               |
| PermissionsApi:BaseUrl                     | URL for downstream [Permissions API](../Saas.Identity/Saas.Permissions/readme.md)                                                       | false  |                                       |
| PermissionsApi:LocalCertificate            | A Base64 encoded certificate (.CER) used to authenticate with the permissions API. Only used for local development.                     | true   |                                       |

### iv. Starting the App

1. Insert secrets marked as required for running locally into your secrets manager (such as by using provided script).
1. Start app. Service will launch as presented Swagger API.

## 3. Additional Resources

### i. LocalDB
If using the LocalDB persistance for local development, tables and data can be interacted with directly through Visual Studio. Under the `View` menu, find `SQL Server Object Explorer`. Additional documentation is available [here](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver16)

Configure the following in your managed secrets to use LocalDB:
>"ConnectionStrings:TenantsContext": "Server=(localdb)\\mssqllocaldb;Database=TenantsContext;Trusted_Connection=True;MultipleActiveResultSets=true"
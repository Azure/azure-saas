# Saas.SignupAdministration.Web

## Module Overview

For a complete overview of this module, please see the [SaaS.SignupAdministration.Web](https://azure.github.io/azure-saas/components/signup-administration/) page in our documentation site.

## How to Run Locally

### Requirements

To run this web app locally, you must have the following installed on your machine:

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

- (Recommended) [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)

You must also have the following:

- A deployed instance of Azure AD B2C (or another identity provider, if you plan on replacing B2C)
  - This can be done by following Step #1 in our [Quick Start](https://azure.github.io/azure-saas/quick-start/#1-setup-identity-framework) guide

### App Settings

In order to run the project locally, the App Settings marked as `secret=true` must first be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). When deployed to azure using the bicep deployments, these secrets are [loaded from Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment) instead.

Default values for non secret app settings can be found in [appsettings.json](Saas.Admin.Service/appsettings.json)

|   | AppSetting Key                        | Description                                           | Secret | Default Value                 |   |
|---|---------------------------------------|-------------------------------------------------------|--------|-------------------------------|---|
|   | ---                                   | ---                                                   | ---    | ---                           |   |
|   | AppSettings:AdminServiceBaseUrl       | URL for downstream admin service                      | false  | https://localhost:7041/       |   |
|   | AppSettings:AdminServiceScopeBaseUrl  | The B2C URL for the admin service scope               | false  |                               |   |
|   | AppSettings:AdminServiceScopes        | List of scopes to authorize user for on the admin service. Space delimited     | false  |                               |   |
|   | AzureAdB2C:Instance                   |                                                       | true   |                               |   |
|   | AzureAdB2C:Domain                     |                                                       | true   |                               |   |
|   | AzureAdB2C:ClientId                   |                                                       | true   |                               |   |
|   | AzureAdB2C:ClientSecret               |                                                       | true   |                               |   |
|   | AzureAdB2C:TenantId                   |                                                       | true   |                               |   |
|   | AzureAdB2C:SignedOutCallbackPath      |                                                       | false  | /signout/B2C_1A_SIGNUP_SIGNIN |   |
|   | AzureAdB2C:SignUpSignInPolicyId       |                                                       | false  | B2C_1A_SIGNUP_SIGNIN          |   |
|   | KeyVault:Url                          | KeyVault URL to pull secret values from in production | false  |                               |   |
|   | AllowedHosts                          |                                                       | false  | *                             |   |
|   | Logging:LogLevel:Default              |                                                       | false  | Information                   |   |
|   | Logging:LogLevel:Microsoft.AspNetCore |                                                       | false  | Warning                       |   |
|   | EmailOptions:EndPoint                 | Service endpoint to send confirmation email           | true   |                               |   | 
|   | EmailOptions:FromAddresss             |                                                       | false  |                               |   |
|   | EmailOptions:Subject                  |                                                       | false  |                               |   |
|   | EmailOptions:Body                     |                                                       | false  |                               |   |
### Starting the App

1. Insert secrets marked as required for running locally into your secrets manager using provided script.
(Instructions for running in visual studio)
 
### JsonSessionPersistenceProvider 
The JsonSessionPersistenceProvider maintains the state of the work flow in the Session, and allows for forward and backward movment in the app with 
access to all of the values of the Tenet. Custom providers can be used as long as they inherit from the IPersistenceProvider interface. 
The 2 methods are: 
        public void Persist(string key, object value);
        public T Retrieve<T>(string key);

In place of the Session you could use an OLDB provider like a MS SQL Percistance Provider. 
=======
1. Insert secrets marked as required for running locally into your secrets manager.
2. Set the start projects: 
    a. Saas.Admin.Service
    b. Saas.Permissions.Api
    c. Saas.Application.Web
        i. This is the BadgeMeUp Website, demo software to sale.  
    d. Saas.SignupAdministration.Web
        i. This is the work flow that adds new subscriptions to 


### Running Tests

### Deploying to Azure

<!-- TODO: Add link to instructions to deploy entire kit -->
You can deploy the entire kit at once using the instructions found [here](readme.md). Deploying the entire kit at once will set up all dependancies correctly and configure the application for you in Azure. It is highly reccomended to deploy the entire solution.

If, for your use case, you would like to deploy just this single module, you may do so by following these instructions:

1. Install the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) and authenticate using the `az login` command

2. [Create a resource group](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-cli#create-resource-groups) in your preferred region

<!-- TODO: Put instructions in for running bicep deploy -->
3. Run bicep deploy command(s)

4. ....
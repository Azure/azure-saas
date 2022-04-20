# SaaS.Admin.Service

The SaaS Admin Service is an API that is reponsible for tenant management operations. Within this folder, you will find 3 sections:

1. Saas.Admin.Service - The .NET Web API project containing the code for the API

2. Saas.Admin.Service.Deployment - The bicep module for deploying the infrastructure required to host the API in Azure

3. Saas.Admin.Service.Tests - Unit tests for the service

## Project Overview

For more information on this module, please see the admin service page on our documentation site [here](https://azure.github.io/azure-saas/components/admin-service/).

## How to Run Locally

### Requirements

To run the web api, you must have the following installed on your machine:

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

- (Reccomended) [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download)

You must also have the following:

- A connection string to a running, empty SQL Server Database
  - Some options for using SQL Server Locally which we reccomend:
    - [Local DB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15) (Windows Only)
    - [SQL Server Docker Container](https://hub.docker.com/_/microsoft-mssql-server)
    - [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### App Settings

In order to run the project locally, the App Settings marked as `secret: true` must be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows). When deployed to azure using the bicep deployments, these secrets are [loaded from Azure Key Vault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment) instead.

Default values for non secret app settings can be found in [appsettings.json](Saas.Admin.Service/appsettings.json)

| AppSetting Key |  Description | Secret | Required (Production) | Required (Locally) |
| ---  | --- | --- | --- | --- |
| AzureAdB2C:Instance | | true | true | true | 
| AzureAdB2C:Domain | | true | true | true |
| AzureAdB2C:ClientId | | true | true | true |
| AzureAdB2C:TenantId | | true | true | true |
| AzureAdB2C:SignedOutCallbackPath | | false | true | true |
| AzureAdB2C:SignUpSignInPolicyId | | false | true | true |
| KeyVault:Url | URL to pull secret values from in production | false | true | false |
| KeyVault:PermissionsApiCertName | URL to pull secret values from in production | false | true | false |
| PermissionsApi:BaseUrl | URL for downstream [Permissions API](../Saas.Permissions/readme.md) | false | true | true
| ConnectionStrings:TenantsContext | Connection String to SQL server database used to store tenants data | true | true | true |
| AllowedHosts | | false | false | false |
| Logging:LogLevel:Default | | false | false | false |
| Logging:LogLevel:Microsoft.AspNetCore | | false | false | false |

### Starting the App

1. Insert secrets marked as required for running locally into your secrets manager using provided script.
(Instructions for running in visual studio)

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
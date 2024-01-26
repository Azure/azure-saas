---
type: docs
title: "SaaS.Permissions.Service"
weight: 55
---

## Overview

The [SaaS.Permissions.Service](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions) module (aka Permissions Service) is a component of the [Identity Framework](../). It is an API that serves 2 main functions: 

1. Handles Create, Read, Update, and Delete (CRUD) operations from the rest of the solution for permission data
2. Serves as an endpoint for the [Identity Provider](../identity-provider) to retrieve permission data in order to enrich the user token with claims

## How to Run Locally

Instructions to get this module running on your local dev machine are located in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions).

### Configuration and Secrets

A list of app settings and secrets can be found in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions). All non-secret values will have a default value in the `appsettings.json` file. All secret values will need to be set using the [.NET Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) when running the module locally, as it is not recommended to have these secret values in your `appsettings.json` file.

When deployed to Azure, the application is configured to load its secrets from [Azure Key Vault](https://docs.microsoft.com/azure/key-vault/general/overview) instead. If you deploy the project using our ARM/Bicep templates from the Quick Start guide, the modules will be deployed to an Azure App Service which accesses the Azure Key Vault using a [System Assigned Managed Identity](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/overview). The Permissions Service module is also configured with [key name prefixes](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix) to only import secrets with the prefix of `permissions-`, as other modules share the same Azure Key Vault.

## Module Design

### Dependencies

- SQL Server Database
- Microsoft Graph API

### Consumers

- [Identity Provider](../identity-provider) (Azure AD B2C)
- [Admin Service](../../../components/admin-service)

### Authentication

The Permissions Service is secured using API Key Authentication. The API Key is set using the `AppSettings:ApiKey` secret and there is middleware on the API that will verify that all incoming requests have an API key that matches on the `x-api-key` header. If you deploy the application following the steps in the [Quick Start](../../../quick-start) guide, an API key is randomly generated for you and uploaded to Azure Key Vault.

### Database

[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) is used to manage the SQL Server Database schema and connections. We are using [Code First Development](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database) and, if no data or schema exists in the database on application startup, the application will automatically create the database schema that is defined in our [model files](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions/Saas.Permissions.Service/Data). If you make any changes to these models, you will need to preform a [migration](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/) to upgrade the database schema.

### Microsoft Graph API

The [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/overview) is an API that provides a unified experience for accessing data on users within a Microsoft Entra or Azure AD B2C tenant. Since we are using Azure AD B2C as our default Identity Provider, we must also use the Graph API when it becomes necessary to fetch data on our users. If you'd like to replace the identity provider with something else, you must also replace the Graph API calls within the permissions service to gather user data. These areas are clearly labeled with comments inline with the code.

### Swagger

The Permissions Service uses [Swashbuckle](https://www.nuget.org/packages/Swashbuckle) to generate the OpenAPI definition and a UI for testing. This definition is also consumed by the Admin Service to generate its client implementation for interfacing with this API. Read more about using it [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio).

## FAQ and Design Considerations

- Permissions are stored in the database in a single table (dbo.Permissions) with 3 pieces of data: Tenant ID, User ID (Email), and PermissionString. All 3 together make the row unique (i.e., you cannot have the same Permission for the same user on the same tenant more than once). Permissions are stored as a string (ex: Admin, User.Read, User.Write) for simplicity and extensibility. You may choose to store these in a separate database table and reference them by ID number if you have a large number of permissions and you want to enforce the types of permissions being assigned.
- We have purposefully chosen to flow all CRUD operations on permissions through the [Admin Service](../../../components/admin-service). This is for a number of reasons:
  1. It removes the burden of authorization from the permissions service. All the permissions service needs to worry about is accepting a valid API Key, which only the identity provider and admin service possess. For higher security applications, you may choose to preform more authorization checks before adding permissions
  2. It simplifies the architecture. The frontend applications do not need to have any knowledge of the permissions service existing. When a tenant is created, the applications make 1 call to the admin service, and it handles the subsequent call to update the permissions records.

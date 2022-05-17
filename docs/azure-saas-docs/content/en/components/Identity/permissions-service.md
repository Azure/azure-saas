---
type: docs
title: "SaaS.Permissions.Service"
weight: 55
---

## Overview

The [SaaS.Permissions.Service](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions) module (aka Permissions Service) is a component of the [Identity Framework](../). It is an API that serves 2 main functions: 

1. Handles CRUD operations from the rest of the solution for permission data
2. Serves as an endpoint for the [Identity Provider](../identity-provider) to retrieve permission data in order to enrich the user token with claims

## How to Run Locally

Instructions to get this module running on your local dev machine are located in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions).

### Configuration and Secrets

A list of app settings and secrets can be found in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions). All non-secret values will have a default value in the `appsettings.json` file. All secret values will need to be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) when running the module locally, as it is not reccomended to have these secret values in your `appsettings.json` file.

When deployed to Azure, the application is configured to load in its secrets from [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) instead. If you deploy the project using our bicep templates from the Quick Start guide, the modules will be deployed to an app service which accesses the key vault using a [System Assigned Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview). The Permissions Service module is also configured with [key name prefixes](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix) to only import secrets with the prefix of `permissions-`, as other modules share the same keyvault.

## Module Design

### Dependencies

- SQL Server Database
- Microsoft Graph API

### Consumers

- [Identity Provider](../identity-provider) (Azure AD B2C)
- [Admin Service](../../components/admin-service)

### Authentication

The Permissions Service is secured using [TLS Mutual Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-6.0#configure-certificate-validation) (Certificate Authentication). The application layer is configured to verify the authenticity of the certificate by comparing the thumbprint of the certificate against a configuration value. For TLS Mutual Authentication to work properly, the web server must also be configured to forward the certificate to the application layer. This is done for you if you deploy the application following the steps in the [Quick Start](../../quick-start) guide, but if you choose to run the code in an environment you provision, you will need to do [this configuration](https://docs.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth) yourself.

> **Important!**  
> The certificate that gets deployed out of the box by following the Quick Start is a self signed certificate. Self signed certificates are not meant for production use, and it highly reccomended that you replace this certificate with one signed by a Certificate Authority before using this module in production.

### Database

[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) is used to manage the SQL Server Database schema and connections. We are using [Code First Development](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database) and, if no data or schema exists in the database on application startup, the application will automatically create the database schema that is defined in our [model files](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions/Saas.Permissions.Service/Data). If you make any changes to these models, you will need to preform a [migration](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/) to upgrade the database schema.

### Microsoft Graph API

The [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/overview) is an API that provides a unified experience for accessing data on users within an AAD or AAD B2C tenant. Since we are using Azure AD B2C as our default Identity Provider, we must also use the Graph API when it becomes neccesary to fetch data on our users. If you'd like to replace the identity provider with something else, you must also replace the Graph API calls within the permissions service to gather user data. These areas are clearly labeled with comments inline with the code.

### Swagger

The Permissions Service uses [Swashbuckle](https://www.nuget.org/packages/Swashbuckle) to generate the OpenAPI definition and a UI for testing. This definition is also consumed by the Admin Service to generate its client implementation for interfacing with this API. Read more about using it [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio).

## FAQ and Design Considerations

- Q: Why did we choose to secure the permissions service with certificate authentication over API Keys/JWT Tokens/Another Method?
  - A: The communication between Azure AD B2C (our default Identity Provider) and the permissions service must be secured with either [Basic or Certificate Auth](https://docs.microsoft.com/en-us/azure/active-directory-b2c/add-api-connector-token-enrichment?pivots=b2c-custom-policy#configure-the-restful-api-technical-profile) and it is not considered best practice to use Basic authentication in a production environment.
- Q: Why did we choose to not give the permissions service it's own key vault?
  - A: For simplicity's sake, we decided to use key name prefixes to store keys for each module across the entire application into one keyvault. This is an acceptable scenario, but for tighter security, you may choose to separate the secrets for each module into a dedicated keyvault.

- Permissions are stored in the database in a single table (dbo.Permissions) with 3 pieces of data: Tenant ID, User ID (Email), and PermissionString. All 3 together make the row unique, ie you cannot have the same Permission for the same user on the same tenant more than once. Permissions are stored as a string (ex: Admin, User.Read, User.Write) for simplicity and extensibility. You may choose to store these in a database table and reference them by ID number if you have a large number of permissions and you want to enforce the types of permissions being assigned.
- We have purposefully chosen to flow all CRUD operations on permissions through the [Admin Service](../admin-service). This is for a number of reasons:
  1. It removes the burden of authorization from the permissions service. All the permissions service needs to worry about is accepting a valid certificate, which only the identity provider and admin service possess. For higher security applications, you may choose to preform more authorization checks before adding permissions
  2. It simplifys the architecture. The frontend applications do not need to have any knowledge of the permissions service existing. When a tenant is created, they make 1 call to the admin service, and it handles the subsequent call to update the permissions records.
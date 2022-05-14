---
type: docs
title: "SaaS.Admin.Service"
weight: 50
---

## Overview

The [SaaS.Admin.Service](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin) module (aka Admin Service) is an API that has two main responsibilities:

1. Preforming CRUD operations on tenants
2. Serving as a broker to the permissions API to assign roles and permissions to tenants

## How to Run Locally

Instructions to get this module running on your local dev machine are located in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin).

### Configuration and Secrets

A list of app settings and secrets can be found in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions). All non-secret values will have a default value in the `appsettings.json` file. All secret values will need to be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) when running the project locally, as it is not reccomended to have these secret values in your `appsettings.json` file.

When deployed to Azure, the application is configured to load in its secrets from [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) instead. If you deploy the project using our bicep templates from the Quick Start guide, the modules will be deployed to an app service which accesses the key vault using a [System Assigned Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview). The Admin Service module is also configured with [key name prefixes](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix) to only import secrets with the prefix of `admin-`, as other modules share the same keyvault.

## Module Design

### Dependencies

- [SaaS.Permissions.Service](../identity/permissions-service)
  - Depends on the SaaS.Permissions.Service for CRUD operations on permissions records
- [Identity Provider](../identity/identity-provider)
  - Depends on the identity provider to authenticate callers via their JWT token

### Consumers

- [SaaS.SignupAdministration.Web](../signup-administration)

- [Saas.Application.Web](../saas-application)

### Authentication

The Admin Service is secured using OAuth 2.0 authentication via the Microsoft Identity Platform. Incoming requests must contain a valid JWT Bearer token on the `Authorization` header. The token must contain a valid scope that the calling application has been authorized to use. To learn more about how this process works and is configured in B2C, we highly reccomend checking out our list of [identity resources & documentation]((../../resources/additional-recommended-resources#identity-focused)).

For authorization, we have also included middleware on the Admin Service that extracts the user's permission records from the JWT token claims and preforms authorization based on policies applied at the route level. In other words, before preforming any action on a tenant once a request is received, the admin service will first ensure that the user making the request has a claim to that tenant on their token. If there is no claim to that tenant, or their role does not match what they're trying to do, the request will be denied.

Implementing this in a multitenant fashion is often the most difficult part of starting a SaaS project, so we tried to make it as extensible as possible to support a wide range of scenarios.

### Database

[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) is used to manage the SQL Server Database schema and connections. We are using [Code First Development](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database) and, if no data or schema exists in the database on application startup, the application will automatically create the database schema that is defined in our [model files](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions/Saas.Permissions.Service/Data). If you make any changes to these models, you will need to preform a [migration](https://docs.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/) to upgrade the database schema.

### Swagger and NSwag

The Admin Service uses [Swashbuckle](https://www.nuget.org/packages/Swashbuckle) to generate the OpenAPI definition and a UI for testing. This definition is also consumed by the Signup Administration site to generate its client implementation for interfacing with this API. Read more about using it [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio).

In addition, it also uses NSwag to consume the OpenAPI definition for the Permissions Service to generate a client implementation for that. Read more about NSwag [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-6.0&tabs=visual-studio).

## FAQ and Design Considerations

- Q: Why did we choose to use strings to store our IDs and not GUIDs?
  - A: The default implementation that we chose is to use GUIDs for IDs, but store them as strings. This decisision was made so that consumers of the ASDK project would not be forced into using GUIDs, should they want to use something else for IDs.
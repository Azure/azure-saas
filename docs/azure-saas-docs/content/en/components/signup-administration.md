---
type: docs
title: "SaaS.SignupAdministration.Web"
weight: 80
---

## Overview

The [SaaS.SignupAdministration.Web](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration) (aka SignupAdmin) module is a web application meant to faciliate self service onboarding to your SaaS solution. End Users/Customers can visit this site to:

- Sign up for an account

- Go through an onboarding flow to create a new tenant

- Manage their existing tenants.

This site also supports administrative functionality for global administrators to view and manage all tenants and users of the application.

## How to Run Locally

Instructions to get this module running on your local dev machine are located in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration).

### Configuration and Secrets

A list of app settings and secrets can be found in the module's [readme.md](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions). All non-secret values will have a default value in the `appsettings.json` file. All secret values will need to be set using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) when running the project locally, as it is not reccomended to have these secret values in your `appsettings.json` file.

When deployed to Azure, the application is configured to load in its secrets from [Azure Key Vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) instead. If you deploy the project using our bicep templates from the Quick Start guide, the modules will be deployed to an app service which accesses the key vault using a [System Assigned Managed Identity](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview). The SignupAdmin module is also configured with [key name prefixes](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#use-a-key-name-prefix) to only import secrets with the prefix of `signupadmin-`, as other modules share the same keyvault.

## Module Design

### Dependencies

### Consumers

### Authentication

using MSAL to auth and get token for admin service

### NSwag


## Design Considerations

For ease of management, we have chosen to incorporate the global administrative functionality into this application. You may choose to separate this functionality into a different application if you require more administrative functionality than just tenant and user management.

> We chose to use NSwag to generate our client implementation for the Admin Service. The NSwag project provides tools to generate OpenAPI specifications from existing ASP.NET Web API controllers and client code from these OpenAPI specifications. This provides us a ready-to-use HTTP client without having to write much boilerplate. Read more about [using NSwag on ASP.NET projects](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-6.0&tabs=visual-studio)


## FAQ and Design Considerations
- saas notifications -- current implementation vs eventing system


## Signup Administration Flows


## Sign In

See the [Sign-In Flow](./Identity/identity-flows.md##Sign-In) in Identity Flows.

## Onboarding Flow

```mermaid
sequenceDiagram  
actor user as Tenant Admin
participant signup as Signup App
participant admin as Admin Api
participant auth as Auth Service
participant perm as Permissions Api
participant email as Email Logic App

user->>signup: Sign Up Button Clicked

signup->>signup: User Signed In/Token Exists?
signup-->>user: No, Redirect to /login
user->>auth : Sign in or Sign Up
auth-->>user : JWT
user->>signup: Sign Up Button Clicked
signup->>signup: Token Exists?
signup->>user : Yes, Start Onboarding Flow -- Org Name Page
user->>signup: Org Name Provided
signup-->>user : Category Select
user->>signup: Select Category
signup-->>user : Route Name Page
signup->>user: Enter Route Name
signup->>admin: Check if Route Exists
admin->>signup: Route does not exist
signup-->>user : Validation Page
user->>signup : Submit
signup->>admin : Create Tenant
admin->>admin : Create Tenant
admin->>perm : Add Admin Permission for Tenant
perm-->>admin : Permission Added
admin->>email : Send Tenant Created Confirmation Email
email-->>admin : Sent
admin-->>signup : Tenant Created
admin-->>user : Tenant Created Confirmation Page
```

## Add New Tenant Admin - Existing User

```mermaid
sequenceDiagram
actor user as Tenant Admin
participant signup as SignupAdministration Site
participant admin as Admin API
participant perm as Permissions API
participant auth as Auth Service (B2C)

user->>signup : Get list of tenants
signup->>admin : Get list of tenants for user
admin-->>signup : List of tenants for user
signup-->>user : List of tenants
user->>signup : Add user to tenant by email
signup->>admin : POST: Add user to tenant by email
admin->>admin : Claim({tenantId}.users.write)
admin->>perm : Add user to tenant by email
perm->>auth : User exists?
auth-->>perm : User exists
perm->>perm : Add Permissions Record
perm-->>admin : Ok
admin-->>signup : Ok
signup-->>user : Ok
```

## Add New Tenant Admin - User Does Not Exist

```mermaid
sequenceDiagram
actor user as Tenant Admin
participant signup as SignupAdministration Site
participant admin as Admin API
participant perm as Permissions API
participant auth as Auth Service (B2C)

user->>signup : Get list of tenants
signup->>admin : Get list of tenants for user
admin-->>signup : List of tenants for user
signup-->>user : List of tenants
user->>signup : Add user to tenant by email
signup->>admin : POST: Add user to tenant by email
admin->>admin : Claim({tenantId}.users.write)
admin->>perm : Add user to tenant by email
perm->>auth : User exists?
auth-->>perm : User does not exist
perm-->>admin : Error, User does not exist
admin-->>signup : Error, User does not exist
signup-->>user : Error, User does not exist    
```
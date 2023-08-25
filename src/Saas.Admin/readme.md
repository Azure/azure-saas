# SaaS Admin Service API

The SaaS Admin Service is an API that is reponsible for tenant management operations. 

This project hosts a service API which serves as a gateway to administrate the SaaS ecosystem of Tenants.

## Overview

Within this folder you will find two subfolders:

- **Saas.Permissions.Service** - the C# project for the API
- **deployment** - a set of tools for deploying the API for production
  - The sub-subfolder **[act](./deployment/act)** is for deploying the API for remote debugging 
- Saas.Admin.Service.Tests - Unit tests for the API.

## Dependencies

The service depends on:

- The **Identity Foundation** that was deployed a spart of the Identity Foundation and on the [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/use-the-api).
- The **[SaaS Permissions Services API](./../Saas.Identity/Saas.Permissions/readme.md)**.
- The [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/overview). 

For a complete overview, please see the [SaaS.Admin.Service](https://azure.github.io/azure-saas/components/admin-service/) page in our documentation site.

## Provisioning the API

To work with the SaaS Admin Services API it must first be provisions to your Azure ASDK resource group. This is true even if you initially is planning to run the API in your local development environment. The provisioning ensure that configuration and settings to be correctly added to your Azure App Configuration store and readies the API for later deployment to Azure.

Provisioning is easy:

1. Navigate to the sub folder `deployment`.

2. Run these commands:

   ```bash
   ./setup.sh
   ./run.sh
   ```

Now you're ready to move on.

## How to Run Locally

Guidelines for getting up and running with SaaS Signup Administration in your local development, are identical to the guidelines found the *[Requirements](./../Saas.Identity/Saas.Permissions/readme.md#Requirements)* and the *[Configuration, settings and secrets when running locally](./../Saas.Identity/Saas.Permissions/readme.md#running-the-saas-permissions-service-api-locally)* section in the [SaaS Permissions Service readme](./../Saas.Identity/Saas.Permissions/readme.md). 

## Running the SaaS Administration Service API Locally

--- TODO BEGIN --- 

*Add some guidelines about how to create valid JWT tokens to test the API locally etc...* 

---TODO END ---

## How to Deploy the SaaS Administration Service API to Azure

The guidelines are identity to *[How to Deploy SaaS Permissions Service API to Azure](./../Saas.Identity/Saas.Permissions/readme.md#how--to-deploy-saas-permissions-service-api-to-azure)*.

## Debugging Remotely in Azure

The guidelines are identity to *[Debugging in Azure](./../Saas.Identity/Saas.Permissions/readme.md#debugging-in-azure)* for the SaaS Permissions Service API.

Happy debugging!
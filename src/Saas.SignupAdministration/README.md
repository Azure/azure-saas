# SaaS Sign-Up Administration Web App

This SaaS Sign-up an Administration Web App is an application for the onboarding and administration of tenants.

The application has been developed in [MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-6.0) format, its pages built by respective Controllers paired to Views. See the Views and Controllers directories for relevant service logic or display logic.

For a complete overview, please see the [SaaS.SignupAdministration.Web](https://azure.github.io/azure-saas/components/signup-administration/) page in our documentation site.

## Overview

Within this folder you will find two subfolders:

- **Saas.SignupAdministration.Web** - the C# project for the web app.
- **deployment** - a set of tools for deploying the web app for production
  - The sub-subfolder **[act](./deployment/act)** is for deploying the web app for remote debugging 

## Dependencies

The service depends on:

- The **Identity Foundation** that was deployed a spart of the Identity Foundation and on the [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/use-the-api).
- The **[SaaS Permissions Services API](./../Saas.Identity/Saas.Permissions/readme.md)**.
- The **[SaaS Administration Service API](./../Saas.Admin/readme.md)**.

## Provisioning the  Web App

To work with the SaaS Signup Administration Web app it must first be provisions to your Azure ASDK resource group. This is true even if you initially is planning to run the API in your local development environment. 

The provisioning ensure that configuration and settings to be correctly added to your Azure App Configuration store and readies the API for later deployment to Azure.

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

## Running the SaaS Sign-up Administration Web App Locally

--- TODO BEGIN --- 

*Needs more investigation.*

*Add some guidelines about how to do this. Probably by leveraging [ngrok](https://ngrok.com/)...* 

---TODO END ---

## How to Deploy the SaaS Administration Service API to Azure

The guidelines are identity to *[How to Deploy SaaS Permissions Service API to Azure](./../Saas.Identity/Saas.Permissions/readme.md#how--to-deploy-saas-permissions-service-api-to-azure)*.

## Debugging Remotely in Azure

The guidelines are identity to *[Debugging in Azure](./../Saas.Identity/Saas.Permissions/readme.md#debugging-in-azure)* for the SaaS Permissions Service API

## Debugging Azure B2C



[Troubleshoot custom policies with Application Insights - Azure AD B2C | Microsoft Learn](https://learn.microsoft.com/en-us/azure/active-directory-b2c/troubleshoot-with-application-insights?pivots=b2c-custom-policy)
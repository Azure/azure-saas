# Saas Application (demo)

This sample default application is **Contoso BadgeMeUp**. It is easy to run the application locally to explore how it works within the context of the other SaaS modules.

![BadgeMeUp Screenshot](badgemeup-screenshot.gif)

Contoso BadgeMeUp is a simple SaaS application that Contoso sells to companies that want a great tool to improve the culture within their organization.

## 1. Module Overview

This project hosts an application for providing the intended SaaS functions to valid Tenants. 

For a design overview, please see the [SaaS.Application.Web](https://azure.github.io/azure-saas/components/saas-application/) page in our documentation site.

The application has been developed using [Razor](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-6.0&tabs=visual-studio) pages with code behind. Automatic reference to the project name has been established through the string resource class (SR.cs) for the scoped CSS reference in _Layout.cshtml. See the Pages and Service directories for relevant display or service logic.

### Customizing the Template

Throughout the project, various "TODO (SaaS)" tags have been added. They are placed around identifiers you will want to alter to suit your individual application if you plan to build off of the template. Either navigate using the todo list in Visual Studio or search for "TODO (SaaS)" to quickly address these areas and find points to begin development.

### Addressing Tenants

Authentication has been implemented utilizing Azure AD B2C and utilizing the same underlying claims as the Signup application. Further, a demonstration of tenant-specific app logic has been implemented as a starting guide to work by. When an authenticated user is signed into the application, you may visit the route path from the site base to fetch public information regarding the tenant and have it displayed. For instance, when you register the route "MySoftwareCompany" during signup and run the app locally, this will look like:

>https://localhost:5000/MySoftwareCompany

## Overview

Within this folder you will find two subfolders:

- **Saas.Application.Web** - the C# project for the web app.
- **deployment** - a set of tools for deploying the web app for production
  - The sub-subfolder **[act](./deployment/act)** is for deploying the web app for remote debugging 

## Dependencies

The service depends on:

- The **Identity Foundation** that was deployed a spart of the Identity Foundation and on the [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/use-the-api).
- The **[SaaS Permissions Services API](./../Saas.Identity/Saas.Permissions/readme.md)**.
- The **[SaaS Administration Service API](./../Saas.Admin/readme.md)**.

## Provisioning the  Web App

To work with the SaaS Application Web app it must first be provisions to your Azure ASDK resource group. This is true even if you initially is planning to run the web app in your local development environment. 

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

## How to Deploy the SaaS Administration Service API to Azure

The guidelines are identity to *[How to Deploy SaaS Permissions Service API to Azure](./../Saas.Identity/Saas.Permissions/readme.md#how--to-deploy-saas-permissions-service-api-to-azure)*.

## Debugging Remotely in Azure

The guidelines are identity to *[Debugging in Azure](./../Saas.Identity/Saas.Permissions/readme.md#debugging-in-azure)* for the SaaS Permissions Service API

## Debugging Azure B2C

### Azure AD B2C
You'll need to configure your B2C instance for an external authentication provider. Additional documentation is available [here](https://azure.github.io/azure-saas/components/identity/).

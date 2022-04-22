---
type: docs
title: "Quick Start"
linkTitle: "Quick Start"
weight: 10
description: "Getting Started with the Azure SaaS Dev Kit"
---

On this page, you will find instructions to run the dev kit in your local environment, how to deploy the solution to Azure, and where to put your application code to customize the solution.

> Tip: If you're new here and want to learn what this is, check out the [welcome page](..)

## 1. Setup Azure AD B2C

This project uses [Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview) for an IDP (Identity Provider). The first step in setting up this project is to configure a new B2C instance to house our local user accounts. If you'd like to use an IDP other than Azure AD B2C, see our document on how to achieve that here **NEED TO WRITE STILL**.

(need to refine steps here)
1. Create new b2c instance
2. Run our script
3. Gather output values
4. Save for later


After finishing the IDP setup, you may choose to either run the project locally first or immediately deploy the solution to Azure.
## 2.a. Running the Dev Kit in your local dev environment

- Install the latest version of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/). You may also use Visual Studio Code, but the solution and projects are targetted at VS2022.
- Clone the repository `https://github.com/Azure/azure-saas.git` on to your dev machine.
- Open the `.sln` in the root of the repository. This solution includes all of the modules.
- Depending on the project you wish to run, you'll need to set some secrets to properly setup authentication with B2C. See the [App Settings](#app-settings) section below

### App Settings
When running the project locally, you will need to set some app settings & secrets manually using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment). When deployed to azure (below), these secrets are automatically configured for you and stored in Azure Key Vault.

View the readme files in each project's directory for a description of the app settings & secrets you'll need to set in order to run the respective project:

- [SaaS.Admin.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin)
- [SaaS.Permissions.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Permissions)
- [SaaS.SignupAdministration.Web Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration)
- [SaaS.Application Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Application)

## 2.b. Deploying to Azure

Deploying to Azure is easy thanks to our pre-configured ARM (Azure Resource Manager) templates.

This button will take you to the Azure portal, passing it the template. You'll be asked a few questions, and then the solution will be up and running in just a few minutes. You will need your B2C configuration values and secrets from step 1. 

[![Deploy to Azure](https://www.azuresaas.net/assets/images/deploy-to-azure.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json)

## Integrating your application

Now that you've seen how to build and run the code locally, and you have a way to deploy your code to Azure in a repeatable and code-first way, you can integrate your own code into the solution.

We've included a basic application within the `Saas.Application.Web` project that demonstrates a SaaS solution called "BadgeMeUp". BadgeMeUp is simply a badge sharing site that *Contoso* (representing your company) can sell to end customers.

> SaaS solutions come in many shapes as sizes. We picked "BadgeMeUp", because it's a fairly simple scenario to understand. [You can read more about this particular SaaS scenario here](../resources/contoso-badgemeup/).

### How does this work?

This solution uses a Bicep template which is checked into source control. Whenever changes are detected, a GitHub pipeline compiles the template into an ARM template.

> What is Bicep?
> Bicep is a domain-specific language (DSL) that uses declarative syntax to deploy Azure resources. In a Bicep file, you define the infrastructure you want to deploy to Azure, and then use that file throughout the development lifecycle to repeatedly deploy your infrastructure. Your resources are deployed in a consistent manner.

## Learn more about SaaS

There are a plethora of resources to help you on your SaaS journey. They're available in the [SaaS Resources section](../resources/saas-resources/).
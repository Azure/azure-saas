---
type: docs
title: "Quick Start"
linkTitle: "Quick Start"
weight: 10
description: "Getting Started with the Azure SaaS Dev Kit"
---

On this page, you will find instructions to run the dev kit in your local environment, how to deploy the solution to Azure, and where to put your application code to customize the solution.

> Tip: If you're new here and want to learn what this is, check out the [welcome page](..)

## 1. Setup Identity Framework

This project uses [Azure Active Directory B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview) for an IDP (Identity Provider). The first step in setting up this project is to configure a new B2C instance to house your local user accounts. You will also need to deploy the [Permissions API](../components/identity/permissions-service), as B2C will have a dependancy on it.

To setup the Identity Framework, we have provided a powershell script [here]() that automates the setup for you. This powershell script will output a parameters file that you'll need to provide when deploying the solution to azure in step 2.b.

After finishing the IDP setup, you may choose to either run the project locally first or immediately deploy the solution to Azure.
## 2.a. Running the Dev Kit in your local dev environment

- Install the latest version of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/). You may also use Visual Studio Code, but the solution and projects are targetted at VS2022.
- Clone the repository `https://github.com/Azure/azure-saas.git` on to your dev machine.
- Open the `.sln` in the root of the repository. This solution includes all of the modules.
- Depending on the project you wish to run, you'll need to set some secrets to properly setup authentication with B2C. See the [App Settings](#app-settings) section below

### App Settings
When running the project locally, you will need to set some app settings & secrets manually using the [.NET secrets manager](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-6.0#secret-storage-in-the-development-environment). When deployed to azure (below), these secrets are automatically configured for you and stored in Azure Key Vault.

Make sure you check out the [readme files](#more-info) in each project's directory for a description of the app settings & secrets you'll need to set in order to run the respective project.


## 2.b. Deploying to Azure - Entire Solution

Deploying to Azure is easy thanks to our pre-configured ARM (Azure Resource Manager) templates.

This button will take you to the Azure portal, passing it the template. You'll be asked a few questions, and then the solution will be up and running in just a few minutes. You will need your B2C configuration values and secrets from step 1.

[![Deploy to Azure](https://www.azuresaas.net/assets/images/deploy-to-azure.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json)

## 2.c. (Advanced) Deploying to Azure - Single Module

If you'd like to use just one (or more) module from the project, we've provided bicep templates to do that as well. In each project directory, you'll find a folder named `{ModuleName}.Deployment` that contains all the bicep code you'll need to deploy just that Module. Please be advised that there are certain dependencies that each module requires in order for it to deploy properly. You may find that you need to edit the bicep templates to match your use case. You will find instructions and a list of dependencies for each module within the [module's readme](#more-info).

## 3. (Optional) Configure Email Provider

If you choose to deploy the entire solution to Azure, one of the things that gets deployed is the SaaS.Notifications module **need page and link**, an Azure Logic App. This logic app has a simple HTTP trigger that the SaaS.SignupAdministration site is configured to make a POST request to in order to send emails (ie, at the end of the tenant creation process). By default, there is no email provider configured and the emails do not actually get sent. If you'd like to enable email sending, you will need to manually configure your email provider connector of choice inside the Logic App. See the instructions [here](components/saas-notifications.md) to get started.

## Integrating your application

Now that you've seen how to build and run the code locally, and you have a way to deploy your code to Azure in a repeatable and code-first way, you can integrate your own code into the solution.

We've included a basic application within the `Saas.Application.Web` project that demonstrates a SaaS solution called "BadgeMeUp". BadgeMeUp is simply a badge sharing site that *Contoso* (representing your company) can sell to end customers.

> SaaS solutions come in many shapes as sizes. We picked "BadgeMeUp", because it's a fairly simple scenario to understand. [You can read more about this particular SaaS scenario here](../resources/contoso-badgemeup/).

### How does this work?

This solution uses a Bicep template which is checked into source control. Whenever changes are detected, a GitHub pipeline compiles the template into an ARM template.

> What is Bicep?
> Bicep is a domain-specific language (DSL) that uses declarative syntax to deploy Azure resources. In a Bicep file, you define the infrastructure you want to deploy to Azure, and then use that file throughout the development lifecycle to repeatedly deploy your infrastructure. Your resources are deployed in a consistent manner.

## More Info

For more information, including deployment instructions, an outline of dependencies, app settings, and more, check out the readme files for each module:

- [SaaS.Admin.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin)
- [SaaS.Permissions.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Permissions)
- [SaaS.SignupAdministration.Web Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration)
- [SaaS.Application Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Application)

## Learn more about SaaS

There are a plethora of resources to help you on your SaaS journey. They're available in the [SaaS Resources section](../resources/saas-resources/).
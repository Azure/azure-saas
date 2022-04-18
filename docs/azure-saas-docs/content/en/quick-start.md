---
type: docs
title: "Quick Start"
linkTitle: "Quick Start"
weight: 10
description: "Getting Started with the Azure SaaS Dev Kit"
---

On this page, you will find instructions to run the dev kit in your local environment, how to deploy the solution to Azure, and where to put your application code to customize the solution.

> Tip: If you're new here and want to learn what this is, check out the [welcome page](..)

## Running the Dev Kit in your local dev environment

- Install the latest version of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/). You may also use Visual Studio Code, but the solution and projects are targetted at VS2022.
- Clone the repository `https://github.com/Azure/azure-saas.git` on to your dev machine.
- Open the `.sln` in the root of the repository. This solution includes all of the modules.
- Depending on which project you want to launch, you'll likely need to configure the `appsettings.json` configuration. Be careful to not check in keys to your source control system.

> Note: See [Design Considerations](../resources/design-considerations/) for recommendations around using [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/).

## App Settings

You can build the code without configuring your application settings, but you won't be able to run the projects without a few key configuration settings.

    Saas.SignupAdministration.Web
        connectionString
    Saas.Application.Web
        AdminServiceUrl

> Note: `Saas.Admin.Service` uses a local database instance and is automatically populated using Entity Framework.

> This solution uses various `appsettings.json` and `appsettings.development.json` to manage deployment settings and other configuration settings. In a true production deployment, you should configure [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to provide a secure and centralized key storage location. This allows applications to not have direct access to confidential keys.

## Deploying to Azure

Deploying to Azure is easy thanks to an ARM (Azure Resource Manager) template.

This button will take you to the Azure portal, passing it the template. You'll be asked a few questions, and then the solution will be up and running in just a few minutes.

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
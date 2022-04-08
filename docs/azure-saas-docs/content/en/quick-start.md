---
type: docs
title: "Quick Start"
linkTitle: "Quick Start"
weight: 10
description: "Getting Started with the Azure SaaS Dev Kit"
---

On this page, you will find instructions to run the dev kit in your local environment, where to put your application code, and how to deploy the solution to Azure.

> Tip: If you're new here and want to learn what this is, check out the [welcome page](..)

## Running the Dev Kit in your local dev environment

- Install the latest version of [Visual Studio 2022](https://visualstudio.microsoft.com/vs/). You may also use Visual Studio Code, but the solution and projects are targetted at VS2022.
- Clone the repository `https://github.com/Azure/azure-saas.git` on to your dev machine.
- Open the `.sln` in the root of the repository. This solution includes all of the modules.
- Depending on which project you want to launch, you'll likely need to configure the `appsettings.json` configuration. Be careful to not check in keys to your source control system.

> Note: See [Design Considerations](../resources/design-considerations/) for recommendations around using [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/).

    App Settings:
        Saas.Admin.Service
            Uses local DB
        Saas.SignupAdministration.Web
            connectionString
        Saas.Application.Web
            AdminServiceUrl


## Integrating your application

## Deploying to Azure

## Learn more about SaaS

There are a plethora of resources to help you on your SaaS journey. They're available in the [SaaS Resources section](../resources/saas-resources/).
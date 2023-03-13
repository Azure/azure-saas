---
type: docs
title: "Quick Start"
linkTitle: "Quick Start"
weight: 10
description: "Getting Started with the Azure SaaS Dev Kit"
---

On this page, you will find instructions for how to run the dev kit in your local environment, how to deploy the solution to Azure, and where to put your application code to customize the solution.

> Tip: If you're new here and want to learn what is Azure SaaS Dev Kit, check out the [welcome page](..)

## Before You begin

Before you begin, you should [fork](https://docs.github.com/en/get-started/quickstart/fork-a-repo) the Azure SaaS Dev Kit GitHub repository to to your own GitHub account to make it your own.

The Azure SaaS Dev Kit provides an excellent starting point for launching your SaaS solution. To truly reap the benefits, it is essential to customize the dev kit to suit your particular requirements. By tailoring the dev kit, you achieve the solution that aligns with your specific objectives.

## Provisioning the Identity Framework

This project uses [Azure Active Directory B2C](https://docs.microsoft.com/azure/active-directory-b2c/overview) for an IdP (Identity Provider). The first step in setting up this project is to configure a new Azure AD B2C instance to house your local user accounts.

To provision the Identity Framework please follow this [readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.IdentityProvider).

## Provisioning the SaaS Permissions API

After provisioning the Identity Framework, you should provision the Azure services and configuration needed for running the [Permissions API](../components/identity/permissions-service) locally or deploying it to Azure.

The Azure AD B2C configuration that was provisioned as part of the Identity Foundation Services relies on the Saas Permissions API to provide permission claims. When a user logs into your SaaS solution, these permissions claims are added to the users JWT access token.

To provision the SaaS Permissions API please follow the [readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.Permissions).

## Provisioning the Saas Admin API

The SaaS Admin API is allowed to write data to the Permissions API. This means that while Azure AD B2C relies on the Permissions API for existing permissions, the SaaS Admin API can also add new permissions to the permissions database. When new permissions are added, they will be included in future claims.

In addition, the SaaS Admin API provides secure endpoints that only allow access to users with the specific permissions defined of each of the endpoints. These permissions match the permissions in the claims that are returned by the Permissions API.

To provision the SaaS Admin API please follow the [readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin).

## Provisioning the Sign-Up Administration Web Application

The Sign-up Administration Web Application provides a UI for adding new SaaS tenant(*) and for managing permissions and users. The Sign-up Administration Web Application relies on the SaaS Admin API to provide read and write access to the permissions database.

To provision the SaaS Sign-up Administration Web Application please follow the [readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration).

> (*) Note that the term *tenant* is overloaded. A SaaS Tenant is not that same as an Azure AD tenant. The SaaS tenant references each instance of your multi-tenanted application.

## Provisoning the Saas Application

The SaaS Application is a sample application that serves as an example of where your SaaS Solution would fit in. It relies on all the other components mentioned here.

To provision the SaaS Application please follow the [readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Application).

## (Optional) Configure Email Provider

The SaaS.Notifications module **need page and link** is an Azure Logic App responsible for generating email notifications. By default, there is no email provider configured. If you'd like to enable email notifications, you will need to manually configure your email provider connector of choice inside the Logic App. See the instructions [here](../components/saas-notifications) to get started.

## Integrating your application

Now that you've seen how to run the code locally as well as deploy your code to Azure (in a repeatable and code-first way), you can integrate your own code into the solution.

We've included a basic application within the `Saas.Application.Web` project that demonstrates a SaaS solution called "BadgeMeUp". BadgeMeUp is a simple badge sharing site that *Contoso* (representing your company) can sell to end customers.

> SaaS solutions come in many shapes and sizes. We picked "BadgeMeUp", because it's a fairly simple scenario to understand. [You can read more about this particular SaaS scenario here](../resources/contoso-badgemeup/).

## More Info

For more information, including deployment instructions, an outline of dependencies, app settings, and more, check out the readme files for each module:

- [SaaS.Admin.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin)
- [SaaS.Permissions.Service Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Permissions)
- [SaaS.SignupAdministration.Web Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration)
- [SaaS.Application Readme](https://github.com/Azure/azure-saas/tree/main/src/Saas.Application)

## Learn more about SaaS

There are a plethora of resources to help you on your SaaS journey. They're available in the [SaaS Resources section](../resources/additional-recommended-resources/).

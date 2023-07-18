---
type: docs
no_list: false
linkTitle: "Introduction"
weight: 0
---
# Welcome to the Azure SaaS Dev Kit!

This kit provides you with a solid start buidling your own cloud-native [Software as a Service (SaaS)](https://azure.microsoft.com/en-us/overview/what-is-saas/).

The Azure SaaS Development Kit is a deployable reference implementation with pre-built modules designed to help you build your own cloud-native SaaS offerings.  It is:

* A [cloud-native](https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/definition) control plane implementation that includes commonly needed SaaS components such as identity, onboarding, tenant management, and permissions.
* [100% open-source code](https://github.com/Azure/azure-saas), allowing you to build by example or modify/extend to be purpose-built for your particular scenario.
* Fully documented, making it clear how the code functions and why key design decisions were made.
* Built using comprehensive CI/CD DevOps deployment scripts to get quickly up and running and ready for customization as the solution grows. 

## Usage Options

How you use the dev kit is up to you!  Here are some ideas to get you started:

* Modernize an existing application to support [full multitenancy](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments) as part of a shift to a SaaS-based business model.
* Develop a new, greenfield SaaS offering.
* Migrate a SaaS offering from another cloud to Azure.

This is not a one-size-fits-all solution. Use as little or as much as you like. It is designed to be both a modular deployable reference implementation and also a reference architecture. You are free to use and change the code contained within this project in any way you'd like (following the terms of the [license](https://github.com/Azure/azure-saas/blob/main/LICENSE)):

* Deploy the entire solution to Azure using the deployment scripts provided and make changes to fit your exact use case, and start building your SaaS application from there.
* Deploy one or more of our modules and hook it into your existing SaaS application.
* Reference our code & architecture and build something entirely custom using the best practices we outlined.
* Or, simply gain inspiration from this codebase.

## Modular Architecture

* An [**Identity Framework**](components/identity), which includes:
  * An [**Identity Provider (Azure AD B2C)**](components/identity/identity-provider/) that provides flexibility for both local accounts and integration with external providers.
  * A [**Permissions Service**](components/identity/permissions-service) API that manages all user permissions and serves to enrich the user tokens returned from the identity provider.
* The [**Signup / Administration**](components/signup-administration/) web site, which includes:
  * A web app where *your customers* view plans and onboard to your solution.
  * A web app where *you* can administer tenants, including removing them and modifying them.
* The [**Admin Service**](components/admin-service), which provides an extensible API to programatically manage CRUD operations on tenants.
* The [**SaaS.Application**](components/saas-application/), which is a sample end user application that you can extend or replace with your own code.

> The Azure SaaS Dev Kit uses a [**fully multitenant deployment**](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments). Multitenancy is a complex topic with many facets, and there is no *one size fits all* approach.
>
> Read more about multitenant architectures considerations and approaches at [https://aka.ms/multitenancy](https://aka.ms/multitenancy).

![](/azure-saas/diagrams/overview.drawio.png)

## Ready to get started?

Check out the [quick start page](quick-start/).

## Additional Recommended Resources

* [Best practices for architecting multi-tenant solutions](https://aka.ms/multitenancy)
* [ISV Considerations for Azure landing zones](https://aka.ms/isv-landing-zones)
* [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)
* [WingTips Tickets SaaS Application](https://docs.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-welcome-wingtip-tickets-app) - Provides details into tradeoffs with various tenancy models within the database layer.
* [Starter web app for SaaS development](https://docs.microsoft.com/en-us/azure/architecture/example-scenario/apps/saas-starter-web-app) - The Azure architecture center reference that this kit is based off of.

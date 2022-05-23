---
type: docs
no_list: false
linkTitle: "Introduction"
weight: 0
---
# Welcome to the Azure SaaS Dev Kit!

[Software as a Service (SaaS)](https://azure.microsoft.com/en-us/overview/what-is-saas/) doesn’t need to be complex and time consuming.

The Azure SaaS Development Kit is a deployable reference implementation of pre-built modules designed to help you launch your SaaS offering faster:

* It includes commonly needed SaaS components that implement features such as identity, onboarding, and tenant management.
* It is [100% open-source code](https://github.com/Azure/azure-saas), allowing you to build by example or modify/extend to be purpose-built for your particular scenario.
* The code is fully documented, making it clear how the code functions, and why key design decisions were made.

## Usage Options

How you use the dev kit is up to you, here are some ideas to get you started:

* Modernizing an existing application to support [full multitenancy](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments) as part of a shift to a SaaS based business model.
* Developing a greenfield SaaS offering for the first time.
* Migrating a SaaS offering from another cloud to Azure.


This is not a one-size-fits-all solution. Use as little or as much as you like. It is designed to be both a modular deployable reference implementation and also a reference architecture. You are free to use and change the code contained within this project in any way you'd like (following the terms of the [license](https://github.com/Azure/azure-saas/blob/main/LICENSE))):

- [**B2C Authentication Service**](/services/b2c-auth-service/) - Provides a flexible identity solution.
- [**Core App**](/services/core-app/)
	- A web app where your customers to view plans and onboard to your solution.
	- Provides you with tenant administration capabilities. (modify/remove/etc.)
- [**Saas.Application**](/services/saas-application/) - A sample application that you can extend or replace with your own code.


* Deploy the entire solution to Azure using the [Bicep](https://docs.microsoft.com/azure/azure-resource-manager/bicep/) templates we provide, make changes to fit your exact use case, and start building your SaaS application from there.
* Deploy one or more of our modules and hook it into your existing SaaS application.
* Reference our code & architecture and build something entirely custom using the best practices we outlined.
* Or, simply gain inspiration from this codebase.

## Modular Architecture

This kit uses a [microservices architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/microservices-architecture) so that each module is self-contained and can be used independently.

* [**Identity Framework**](components/identity)
  * [**Identity Provider (Azure AD B2C)**](components/identity/identity-provider/) - An identity solution that provides flexibility for both local accounts and integration with external providers.
  * [**Permissions Service**](components/identity/permissions-service) - An API that manages all user permissions and serves to enrich the user tokens returned from the identity provider.
* [**Signup / Administration**](components/signup-administration/)
  * A web app where your customers view plans and onboard to your solution.
  * Provides you with tenant administration capabilities such as modify, remove, etc.
* [**Admin Service**](components/admin-service) - An extensible API to programatically manage CRUD operations on tenants.

* [**SaaS.Application**](components/saas-application/) - A sample end user application that you can extend or replace with your own code.

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
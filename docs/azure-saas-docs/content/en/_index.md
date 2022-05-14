---
type: docs
no_list: false
linkTitle: "Introduction"
weight: 0
---
# Welcome to the Azure SaaS Dev Kit!

[Software as a Service (SaaS)](https://azure.microsoft.com/en-us/overview/what-is-saas/) doesn’t need to be complex and time consuming.

The Azure SaaS Development Kit is a deployable reference implementation of pre-built resources designed to help you launch your SaaS offering faster:

* There are standard SaaS components that implement features such as identity, onboarding, and tenant management.
* 100% [Open-source code](https://github.com/Azure/azure-saas), allowing developers to build by example or modify/extend to be purpose-built for your particular scenario.
* Fully documented code makes it clear how the code functions, and how key decisions were made.

## Who should use this dev kit?

We think this kit can provide value in some way to a large number of people in a varying degree of situations, but we had the following scenarios especially in mind when building this project:

* Modernizing an existing application to support [full multitenancy](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments) as part of a shift to a SaaS based business model
* Developing a greenfield SaaS offering for the first time
* Migrating a SaaS offering from another cloud to Azure
  
If your company/team does not fit in one of these scenarios, that's okay! Check out the project anyway and you may find something that could be of use to you.

## How can this dev kit be used?

The Azure SaaS Dev Kit is designed to be both a modular deployable reference implementation and also a reference architecture. You are free to use and change the code contained within this project in any way you'd like, but we have 3 main ways outlined that we see a team choosing to use this:

1. Deploy the entire solution to Azure using the bicep templates we provide, make changes to fit your exact use case, and start building your SaaS application in the stub app we provide

2. Deploy one or more of our modules and hook it into your existing SaaS application

3. Reference our code & architecture and build something entirely custom using the best practices we outlined

## Modular Architecture

This kit uses a [microservices architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/microservices-architecture) so that each module is self-contained and can be used idependantly.

* [**Identity Framework**](components/identity)
  * [**Identity Provider (B2C)**](components/identity/identity-provider/) - Provides a flexible identity solution.
  * [**Permissions Service**](components/identity/permissions-service) - An API that manages all user permissions and serves to enrich the user tokens returned from the identity provider.
* [**Signup / Administration**](components/signup-administration/)
  * A web app where your customers to view plans and onboard to your solution.
  * Provides you with tenant administration capabilities. (modify/remove/etc.)
* [**Admin Service**](components/admin-service) - An extensible API to programatically manage CRUD operations on tenants.

* [**SaaS.Application**](components/saas-application/) - A sample end user application that you can extend or replace with your own code.

> The Dev Kit uses a [**fully multitenant depoyment**](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments). Multitenancy is a complex topic with many facets, and there is no *one size fits all* approach.
>
> Read more about multitenant architectures and considerations on Azure [here](http://aka.ms/multitenancy).

![](/azure-saas/diagrams/overview.drawio.png)

## Ready to get started?

Check out the [quick start page](quick-start/).

## Additional Recommended Resources

* [Best practices for architecting multi-tenant solutions](https://aka.ms/multitenancy)
* [ISV Considerations for Azure landing zones](https://aka.ms/isv-landing-zones)
* [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)
* [WingTips Tickets SaaS Application](https://docs.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-welcome-wingtip-tickets-app) - Provides details into tradeoffs with various tenancy models within the database layer.
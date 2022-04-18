---
type: docs
no_list: false
linkTitle: "Introduction"
weight: 0
---
# Welcome to the Azure SaaS Dev Kit

[Software as a Service (SaaS)](https://azure.microsoft.com/en-us/overview/what-is-saas/) doesn’t need to be complex and time consuming.

The Azure SaaS Development Kit is a reference implementation of pre-built resources to help you launch your SaaS offering faster:

* There are standard SaaS components that implement features such as identity, onboarding, and tenant management.
* 100% [Open-source code](https://github.com/Azure/azure-saas), allowing developers to build by example or modify/extend to be purpose-built for your particular scenario.
* Fully documented code makes it clear how the code functions, and how key decisions were made.

Whether you're modernizing an existing application, building a new application, or migrating your application, the SaaS dev kit can help you.

## Modular Architecture

This kit uses a [microservices architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/microservices-architecture) so that each module is self-contained and can be used idependantly.

- [**Identity Provider (B2C)**](components/identity/identity-provider/) - Provides a flexible identity solution.
- [**Signup / Administration**](components/signup-administration/)
	- A web app where your customers to view plans and onboard to your solution.
	- Provides you with tenant administration capabilities. (modify/remove/etc.)
- [**Admin Service**](components/admin-service) - An extensible API to programatically manage CRUD operations on tenants. 
- [**Permissions Service**](components/identity/permissions-service) - An API that manages all user permissions and serves to enrich the user tokens returned from the identity provider.
- [**SaaS.Application**](components/saas-application/) - A sample end user application that you can extend or replace with your own code.

> The Dev Kit uses a [**fully multitenant depoyment**](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#fully-multitenant-deployments). Multitenancy is a complex topic with many facets, and there is no *one size fits all* approach.
>
> Read more about multitenant architectures and considerations on Azure [here](http://aka.ms/multitenancy).

![](architecture.drawio.png)

## Ready to get started?

Check out the [quick start page](quick-start/).

## Additional Recommended Resources

* [Best practices for architecting multi-tenant solutions](https://aka.ms/multitenancy)
* [ISV Considerations for Azure landing zones](https://aka.ms/isv-landing-zones)
* [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)
* [WingTips Tickets SaaS Application](https://docs.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-welcome-wingtip-tickets-app) - Provides details into tradeoffs with various tenancy models within the database layer.
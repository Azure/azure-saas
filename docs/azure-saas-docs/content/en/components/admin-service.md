---
type: docs
title: "Admin API"
---

The Admin Service has two main responsibilities: 
1. Preforming CRUD operations on tenants
2. Serving as a broker to assign roles and permissions to tenants. 

## Running Locally

Instructions to get this module running on your local dev machine are located here: https://github.com/Azure/azure-saas/tree/main/src/Saas.Admin

## Dependencies
- SaaS.Identity.Service

## Consumers
- SaaS.SignupAdministration.Web
- Saas.Application.Web


Claims authorization is preformed at the Admin API using middleware.
When the service receives a request to add user roles to a particular tenant, the admin service is first responsible for ensuring the user has a claim to that tenant 
before forwarding the request to the Identity API. 

## Entity Framework

This project uses Entity Framework migrations to manage the database schema and changes that occur between versions while in developoment.

To learn more about migrations:
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

Migrations tutorial: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-6.0

## Design Considerations

- The API uses [Swashbuckle](https://www.nuget.org/packages/Swashbuckle) to generate the OpenAPI definition and a UI for testing. This definition is also consumed by the Signup Administration site to generate their client implementation for interfacing with this API. Read more about it [here](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-6.0&tabs=visual-studio)
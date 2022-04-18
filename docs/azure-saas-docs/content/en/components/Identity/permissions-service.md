---
type: docs
title: "Permissions Service"
weight: 55
---

## Running Locally

<!-- TODO: Need to update this link at some point -->

Instructions to get this module running on your local dev machine are located here: https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity 

The permissions service is an API that is responsible for CRUD operations on users permissions. 

The API uses Entity Framework to manage the SQL Server Connection and Schema. 

## Dependencies:

- SQL Server Database

This API is restricted and can only be called by the following consumers:

## Consumers

- Identity Provider (Azure AD B2C)
- Admin Service

## Authentication

This API is secured with 


https://github.com/AzureAD/microsoft-identity-web/wiki/b2c-limitations


## Data Storage
Entity Framework Core is used to manage the database schema and connections.
## Design Considerations

- Permissions are stored in the database in a single table (dbo.Permissions) with 3 pieces of data: Tenant ID, User ID (Email), and PermissionString. All 3 together make the row unique, ie you cannot have the same Permission for the same user on the same tenant more than once. Permissions are stored as a string (ex: Admin, User.Read, User.Write) for simplicity and extensibility. You may choose to store these in a database table and reference them by ID number if you have a large number of permissions and you want to enforce the types of permissions being assigned.
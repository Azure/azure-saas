---
type: docs
title: "Admin Service"
---
# Admin Service
The Admin Service has two main responsibilities: 
1. Preforming CRUD operations on tenants
2. Serving as a broker to assign roles and permissions to tenants. 

## Dependencies
- SaaS.Identity.Service

## Consumers
- SaaS.SignupAdministration.Web
- Saas.Application.Web
  

Claims authorization is preformed at the Admin API using middleware.
When the service receives a request to add user roles to a particular tenant, the admin service is first responsible for ensuring the user has a claim to that tenant before forwarding the request to the Identity API. 


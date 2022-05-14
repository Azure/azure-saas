---
type: docs
title: "Caveats & Limitations"
weight: 100
---

**Caveat**: This reference implementation does not provide support for per-tenant “local” users (i.e., all users will be accessing all tenants, there will be no landon@tenant1 vs landon@tenant2, it will be just landon@email.com).

**Caveat**: This version only supports “local” users and social identities and does not provide support for configuring federation with other Identity Providers.  

**Caveat**: Even if future versions provide support for configuring federation with additional Identity Providers (IdP), it will not provide support for “per-tenant federation” (i.e., where each tenant could bring their own IdP). This is primarily due to limitations in Azure AD B2C which make it additional work to manage “per-tenants” users/policies within a directory:
* Each federation is configured via directory-wide policy and there is a limit of 200 policies on a directory.

**Caveat**: If all tenants can be assumed to have their own Azure Active Directory (regular B2B), then per-tenant federation could be implemented using Azure AD (multitenant) federation identity provider with “application code” doing the authorization based on specific tenant id claim.

However, if each tenant wants to be able to configure their own completely different IdP (e.g., Okta, Ping, Auth0, Cognito), it would require additional work due to policy limits.



https://github.com/AzureAD/microsoft-identity-web/wiki/b2c-limitations
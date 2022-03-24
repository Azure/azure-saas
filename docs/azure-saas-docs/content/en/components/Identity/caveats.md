---
type: docs
title: "Caveats & Limitations"
weight: 100
---

**Caveat**: ASDK will not provide support for per-tenant “local” users (i.e., all users will be accessing all tenants, there will be no landon@tenant1 vs landon@tenant2, it will be just landon@email.com). 

**Caveat**: Initial version of ASDK will only support “local” users and maybe social identities and will not provide support for configuring federation with other Identity Providers.  

**Caveat**: Even if future versions of ASDK provide support for configuring federation with additional Identity Providers (IdP), it will not provide support for “per-tenant federation” (i.e., where each tenant could bring their own IdP). 

This is primarily due to limitations in Azure AD B2C which make it additional work to manage “per-tenants” users/policies within a directory:
* Each federation is configured via directory-wide policy and there is a limit of 200 policies on a directory.

**Caveat**: If all tenants of ASDK can be assumed to have their own Azure Active Directory (regular B2B), then per-tenant federation could be implemented using Azure AD (multitenant) federation identity provider with “application code” doing the authorization based on specific tenant id claim.

However, if each tenant wants to be able to configure their own completely different IdP (e.g., Okta, Ping, Auth0, Cognito), it would require additional work due to policy limits.
---
type: docs
title: "Caveats & Limitations"
weight: 100
---

There are a few caveats of the identity solution provided within this reference implementation that you should be aware of.

- This reference implementation does not provide support for per-tenant “local” users. All users will be stored in a single Azure AD B2C tenant and the user objects themselves will not be separated by the tenant they signed up with. For example, if you had `jill@contoso.com` sign up to tenant 1, they would also be able to sign into tenant 2, tenant 3, and tenant 4 with the same `jill@contoso.com` account. You may still control what tenants they have roles under via the permissions that come back in their JWT claims.

- The current version only supports “local” users and social identities and does not provide support for configuring federation with other Identity Providers.

- Future versions will likely not provide support for “per-tenant federation” (i.e., where each tenant could bring their own IdP). This is primarily due to limitations in Azure AD B2C which introduce significant overhead when attempting to manage “per-tenants” users & policies within a directory.
  - It is possible, but each federation is configured via directory-wide policy and there is a limit of 200 policies on a directory.
  - If all tenants can be assumed to have their own Azure Active Directory (regular B2B), then per-tenant federation could be implemented using Azure AD (multitenant) federation identity provider with the application code doing the authorization based on specific tenant id claim. However, if each tenant wants to be able to configure their own completely different IdP (e.g., Okta, Ping, Auth0, Cognito), it would require additional work due to policy limits.

- Azure AD B2C has a documented limitation preventing API chaining via the OAuth 2.0 On-Behalf-Of flow. You may request a token to call an API via a web app, but not an API via an API. See the [Azure AD B2C Limitations](https://github.com/AzureAD/microsoft-identity-web/wiki/b2c-limitations) page for more information.
---
type: docs
title: "Identity Framework"
linkTitle: "Identity Framework"
weight: 30
---

The goal of our identity and authorization strategy is to enable us to easily authenticate users and provide basic Role Based Access Control (RBAC) for entities created within the application.

> Recommended Reading:
> * [What is Azure Active Directory B2C?](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview)
> * [Architect multitenant solutions on Azure](http://aka.ms/multitenancy)

## Overview

Our Identity Framework is comprised of two main pieces:

1. Identity Provider (IdP) - Performs user login/authentication and provides a JWT token to the web applications. ASDK comes with Azure AD B2C implemented as the IdP out of the box.

2. Permissions Service - A microservice that tracks what tenants and data each user has access to and serves as an endpoint for the IdP to enrich the user's token with permissions and role claims during the login flow.

The Identity Framework also has a dependency on the [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/overview), which we use to look up user information when needed.

## Architecture Diagram
![Identity Diagram](/azure-saas/diagrams/identity-diagram.drawio.png)
## Sign Up

> See the [Sign Up](./identity-flows/#sign-up) flow under *Identity Flows*

Upon clicking the signup button in either the SignupAdministration site or the end user application, the user is redirected to an Azure AD B2C hosted signup page. After providing the necessary information and submitting the signup form, Azure AD B2C will create the account and redirect the user back to the originating application with a JWT token.

## Sign In

> See the [Sign In](./identity-flows/#sign-in) flow under *Identity Flows*

Upon clicking the Sign In button in either the SignupAdministration site or the end user application, the user is redirected to an Azure AD B2C hosted signup page. After successfully authenticating (either with a local or federated account), Azure AD B2C makes a call to a route on the [Permissions Service](permissions-service/) to retrieve role and permissions information. Upon receiving this data, Azure AD B2C injects data in the form of custom claims and redirects the user back to the originating application with a JWT token.

## More Identity Topics

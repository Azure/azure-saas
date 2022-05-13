---
type: docs
title: "Identity Framework"
linkTitle: "Identity Framework"
weight: 30
---

The goal of our identity and authorization strategy is to enable us to easily authenticate users and provide basic RBAC for entities created within the application.

> Recommended Reading:
> * [What is Azure Active Directory B2C?](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview)
> * [Architect multitenant solutions on Azure](http://aka.ms/multitenancy)

## Overview

Our Identity Framework is comprised of two main pieces:

1. Identity Provider (IDP) - Preforms user login and authorization and provides a JWT token to the web applications. ASDK comes with Azure AD B2C implemented as the IDP out of the box.

2. Permissions Service - A microservice that tracks what data each user has access to, and serves as an endpoint for the IDP to enrich the user's token with permissions and role claims.

The Identity Framework also has a dependency on the [Microsoft Graph API](https://docs.microsoft.com/en-us/graph/overview), which we use to look up certain user information when needed.

## Architecture Diagram
![Identity Diagram](/azure-saas/diagrams/identity-diagram.drawio.png)
## Sign Up

> See the [Sign Up](./identity-flows/#sign-up) flow under *Identity Flows*

Upon clicking the signup button in either the SignupAdministration site or the end user application, the user is redirected to an Azure B2C hosted signup page. After providing the neccesary information and submitting the signup form, Azure B2C will create the account and redirect the user back to the originating application with a JWT token.

## Sign In

> See the [Sign In](./identity-flows/#sign-in) flow under *Identity Flows*

Upon clicking the Sign In button in either the SignupAdministration site or the end user application, the user is redirected to an Azure B2C hosted signup page. After successfully authenticating (either with a local or federated account), Azure B2C makes a call to a route on the [Permissons Service](permissions-service/) to retreive role and permissions information. Upon receiving this data, B2C injects data in the form of custom claims, and redirects the user back to the originating application with a JWT token.

## More Identity Topics

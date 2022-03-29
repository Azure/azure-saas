---
type: docs
title: "Identity Framework"
linkTitle: "Identity Framework"
weight: 30
description: "Overview of identity and authorization."
---

The goal of our identity and authorization strategy is to enable us to easily authenticate users and provide basic RBAC for entities created within the application.

> Recommended Reading:
> * [What is Azure Active Directory B2C?](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview)
> * [Architect multitenant solutions on Azure](http://aka.ms/multitenancy)

## Overview

## Sign Up

Upon clicking the signup button in either the SignupAdministration site or the end user application, the user is redirected to an Azure B2C hosted signup page. After providing the neccesary information and submitting the signup form, Azure B2C will create the account and redirect the user back to the originating application with a JWT token. 

## Sign In

Upon clicking the Sign In button in either the SignupAdministration site or the end user application, the user is redirected to an Azure B2C hosted signup page. After successfully authenticating (either with a local or federated account), Azure B2C makes a call to a route on the [Permissons Service](permissions-service.md) to retreive role information. Upon receiving this data, B2C injects data in the form of custom claims, and redirects the user back to the originating application with a JWT token.

## More Identity Topics

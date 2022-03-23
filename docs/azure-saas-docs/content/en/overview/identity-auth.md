---
type: docs
title: "Identity & Authorization"
linkTitle: "Identity & Authorization"
weight: 50
description: "Overview of identity and authorization."
---

The goal of our identity and authorization strategy is to enable us to easily authenticate users and provide basic RBAC for entities created within the application. 


## Sign Up
Upon clicking the sign up button on either application, a request is submitted to the identity api. The identity API returns a redirect URL for the appropriate b2c sign up page, and the user is redirected to the sign up page hosted by B2C. Upon successfully signing up, the user is redirected back to the identity API. The Identity api verifies permissions and adds custom claims to the user's JWT and then finally redirects once more back to the originating application with a valid JWT token complete with claims. 

## Sign In


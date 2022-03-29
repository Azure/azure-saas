---
type: docs
title: "Identity Flows"
weight: 60
---

## Sign Up

```mermaid
sequenceDiagram  
actor user 
participant frontend as Frontend Application
participant auth as Auth Service (B2C)


user->>frontend : Register Button (/register)
frontend-->>user : Redirect to B2C Hosted Sign Up Page
user->>auth : Sign Up Submitted
auth->>auth : Create Account
auth-->>user : Redirect with JWT
```

## Sign In

```mermaid
sequenceDiagram  
actor user 
participant frontend as Signup Administration App
participant auth as Auth Service (B2C)
participant perm as Permissions API


user->>frontend : Login Button (/login)
frontend-->>user : Redirect to B2C Hosted Sign In Page
user->>auth : Login Submitted
auth->>perm : Get Permissions & Roles
perm-->>auth : Permissions & Roles
auth->>auth : Add Custom Claims to JWT
auth-->>user : Redirect with JWT
```

## Add Permissions Record

This flow is not comprehensive. It only shows the flow from the point of which the Admin API receives a request and forward.

```mermaid
sequenceDiagram
participant frontend as Frontend Application
participant admin as Admin API
participant perm as Permissions API

frontend->>admin : Add Tenant Permission for User
admin->>admin : User Admin of Tenant?
admin->>perm : Add Tenant Permission for User
perm->>perm : Caller Admin API?
perm->>perm : Permission Added in DB
perm-->>admin : Ok
admin-->>frontend : Ok

```

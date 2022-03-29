---
type: docs
title: "Signup Administration"
---


## Design Considerations
## Running Locally

Instructions to get this module running on your local dev machine are located here:
https://github.com/Azure/azure-saas/tree/main/src/Saas.SignupAdministration



## Signup Administration Flows


## Sign In

## Onboarding Flow

```mermaid
sequenceDiagram  
actor user 
participant signup as Signup App
participant admin as Admin Api
participant auth as Auth Service
participant perm as Permissions Api
participant email as Email Logic App

user->>signup: Sign Up Button Clicked

signup->>signup: User Signed In/Token Exists?
signup-->>user: No, Redirect to /login
user->>auth : Sign in or Sign Up
auth-->>user : JWT
user->>signup: Sign Up Button Clicked
signup->>signup: Token Exists?
signup->>user : Yes, Start Onboarding Flow -- Org Name Page
user->>signup: Org Name Provided
signup-->>user : Category Select
user->>signup: Select Category
signup-->>user : Route Name Page
signup->>user: Enter Route Name
signup->>admin: Check if Route Exists
admin->>signup: Route does not exist
signup-->>user : Validation Page
user->>signup : Submit
signup->>admin : Create Tenant
admin->>admin : Create Tenant
admin->>perm : Add Admin Permission for Tenant
perm-->>admin : Permission Added
admin->>email : Send Tenant Created Confirmation Email
email-->>admin : Sent
admin-->>signup : Tenant Created
admin-->>user : Tenant Created Confirmation Page
```

## Add New Tenant Admin - Existing User

```mermaid
sequenceDiagram
actor user
participant signup as SignupAdministration Site
participant admin as Admin API
participant perm as Permissions API
participant auth as Auth Service (B2C)

user->>signup : Get list of tenants
signup->>admin : Get list of tenants for user
admin-->>signup : List of tenants for user
signup-->>user : List of tenants
user->>signup : Add user to tenant by email
signup->>admin : POST: Add user to tenant by email
admin->>admin : Claim({tenantId}.users.write)
admin->>perm : Add user to tenant by email
perm->>auth : User exists?
auth-->>perm : User exists
perm->>perm : Add Permissions Record
perm-->>admin : Ok
admin-->>signup : Ok
signup-->>user : Ok
```

## Add New Tenant Admin - User Does Not Exist

```mermaid
sequenceDiagram
actor user
participant signup as SignupAdministration Site
participant admin as Admin API
participant perm as Permissions API
participant auth as Auth Service (B2C)

user->>signup : Get list of tenants
signup->>admin : Get list of tenants for user
admin-->>signup : List of tenants for user
signup-->>user : List of tenants
user->>signup : Add user to tenant by email
signup->>admin : POST: Add user to tenant by email
admin->>admin : Claim({tenantId}.users.write)
admin->>perm : Add user to tenant by email
perm->>auth : User exists?
auth-->>perm : User does not exist
perm-->>admin : Error, User does not exist
admin-->>signup : Error, User does not exist
signup-->>user : Error, User does not exist                   bb    
```
---
type: docs
title: "Identity Provider"
weight: 50
---
## Overview

For the identity framework, we chose to use  [Azure B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview) as our default identity provider. If you'd like to use another identity provider such as Azure AAD or a different 3rd party tool, you can swap it out. We have a default configuration 

### What does B2C Give Us?

Azure Active Directory B2C provides business-to-customer identity as a service. It enables easily authenticate users to your application using their preferred identity provider, and is configurable to support a wide array of scenarios.

### Configuration

B2C has two methods of configuring the business logic that users follow to gain access to your application: [User Flows and Custom Policies](https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview). User Flows are predefined and are configured directly through the B2C Web Portal. Custom Policies are XML based configuration that is uploaded to the B2C tenant.

The ASDK project uses Custom Policies to configure the B2C tenant. The XML configuration that gets deployed can be found under the [Saas.IdentityProvider](https://github.com/Azure/azure-saas/tree/main/src/Saas.Identity/Saas.IdentityProvider) folder within the repo, and you can read more about how to configure custom policies [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview).

When deploying the B2C Identity Provider via the instructions found in the [Quick Start](../../quick-start) guide, B2C is configured to do the following:

- Provide a hosted SignIn and SignUp page that users can be directed to
- Reach out to the [SaaS.Permissions.Service](../permissions-service) upon a user signing in to fetch their application permissions and roles

You can change/extend the behavior of B2C to do things like collect more information during signup, force users to enroll in Multi-Factor Authentication (MFA), and much more by modifying the custom policies.

### App Roles and Global Admin

We are using [App Roles](https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-add-app-roles-in-azure-ad-apps) to grant users "Global Admin" capabilites for the application. This App Role should only be granted to staff users that need it to administrate ALL the tenants across the entire SaaS solution. These roles are stored directly in B2C and are returned in the JWT token claims when the user signs in.

**How do I add a user to the default Global Admin role?**

If you followed our steps in the [Quick Start](../../quick-start), the user that created the B2C tenant will automatically be added to this global admin role. Follow these steps if you'd like to add additional users:

1. Switch to your B2C Tenant in the Azure portal

2. Navigate to the Azure Active Directory [menu](https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Overview)

3. Click "Enterprise Applications"

4. Under the "Application Type" dropdown, select "All Applications" and click "Apply"

5. Select the `asdk-admin-api` app ![enterprise apps](/azure-saas/images/aad-enterprise-apps.png)

6. Select "Users and Groups" from the menu on the left

7. Click "Add user/group" ![Add user/groups](/azure-saas/images/aad-enterprise-apps-users-groups.png)

8. Select the users you'd like to add to the app role

9. Click "Assign"

10. Repeat steps  5-9, but on the `asdk-b2c-web` app instead

## Design Considerations and FAQ

- Q: Why did we choose b2c?
  - A: We chose B2C because, in additional to authenticating with "local" accounts, it can be easily extended to support a wide array of other identity providers such as Azure AD, Github, and many more. See the [documentation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/add-identity-provider) for details.

- Q: Why did we choose custom policies over user flows?
  - A: User Flows are predefined and meant for more basic use cases. Custom Policies provide more support for automating the setup and deployment of the B2C configuration, and generally provide greater extensibility in the long term for more complicated scenarios.

- Q: Why didn't we use App Roles for all permissions? Why did we choose to put the tenant permissions in a special API?
  - A: App roles in Azure AD B2C are nice, but too many of them get extremely complicated to manage. You can absolutely achieve the same thing using just app roles, but we wouldn't reccomend it if you are going to have more than just a handful of tenants. 

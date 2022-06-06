# Saas.Application.Web

## 1. Introduction
This project is a sample tenant-aware application. *This is where you'll put your own code*.

This sample default application is **Contoso BadgeMeUp**. It is easy to run the application locally to explore how it works within the context of the other SaaS modules.

![BadgeMeUp Screenshot](badgemeup-screenshot.gif)

Contoso BadgeMeUp is a simple SaaS B2B application that Contoso sells to companies that want a great tool to improve the culture within their organization.

## 2. Starting Development

### i. Customizing the Template

Throughout the project, various "TODO (SaaS)" tags have been added. They are placed around identifiers you will want to alter to suit your individual application if you plan to build off of the template. Either navigate using the todo list in Visual Studio or search for "TODO (SaaS)" to quickly address these areas and find points to begin development.

### ii. Addressing Tenants

Basic authentication has been implemented utilizing Azure AD B2C and utilizing the same underlying claims as the Signup application. Further, a demonstration of tenant-specific app logic has been implemented as a starting guide to work by. When an authenticated user is signed into the application, you may visit the route path from the site base to fetch public information regarding the tenant and have it displayed. For instance, when you register the route "MySoftwareCompany" during signup and run the app locally, this will look like:

>https://localhost:5000/MySoftwareCompany

## 3. Additional Resources

* You'll need to configure your B2C instance for an external authentication provider. Additional documentation is available [here](https://docs.microsoft.com/en-us/azure/active-directory-b2c/identity-provider-azure-ad-multi-tenant?pivots=b2c-user-flow).
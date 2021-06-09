# Azure SaaS Development Kit

The Azure SaaS Development Kit provides tools to help developers deliver their applications as a service. The toolkit includes recommended patterns and practices around SaaS platform architecture, onboarding new tenants, automated deployments, operational architecture, security and everything else you need to know to begin building SaaS solutions on the Azure PaaS and Serverless platform. Technologies include: Azure App Service, Azure Web Apps, Azure API Apps, Azure Functions, ASP.NET, Azure REST API, Azure Resource Manager (ARM), Azure Role Based Access Control (RBAC), CI/CD with Azure DevOps, Azure SQL and Azure Storage.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Provider%2FSaas.Provider.Web.Deployment%2Fazuredeploy.json)

## Features and Components

### Architecture
- SaaS Microservice Architecture

### Compute
- App Service Plan

### Presentation
- Web App: Provider Web App
	- Onboarding Flow
	- Tenant Resolutions Options
	- Tenant Administration for Tenant Roles (Owner, Administrator, etc.)
- Web App: SaaS Service Administration

### Identity
- ASP.NET Core Identity
- Azure AD B2C
- Azure AD B2B
- Azure AD

### Logic
- API App: Onboarding API
- API App: Orders API
- API App: Customers API
- API App: Billing API
	- Stripe integration

### Notifications
- API App: Push Notifications API
- Function App: Notifications API
	- SendGrid integration for Email notifications
	- Twilio integration for SMS notifications
- Logic App: New User notifications
	- SQL Connector

### Data Access
- Entity Framework Core Data Access solution

### Data
- Azure SQL Server: Hosting off all Azure SQL Databases
- Azure SQL Server: Elastic Pooling, Elastic Jobs and Elastic Queries
- Azure SQL Database: Tenant databases supporting both Single and Multitenant scenarios
- Azure SQL Database: Catalog Database
- Azure Cosmos DB: Onboarding Flow datastore

### Storage
- Web Deployment Packages for all solutions components

### Monitoring
- Azure Application Insights included for all Web and API Apps

### Deployment
- Simplified deployment with 'Deploy to Azure' for each microservice
- Azure Resource Manager (ARM) Templates for each microservice

## Solution Roadmap
- Azure Kubernetes Services (AKS) for tenant containerization
- Azure Container Registry (ACR)
- Azure Bicep for Azure resource deployments
- Xamarin Cross Platform Mobile Apps - Multitenant

## Production SaaS Solution
Subscriber from Modern Appz is a live production SaaS solution built entirely on the Azure SaaS Development Kit:
https://www.onsubscriber.com

## Subscribe for Updates
Subscribe for notifications of updates and new features: 
https://www.onsubscriber.com/azuresaas

## Subscribe for additional SaaS content 
https://www.onsubscriber.com/saasacademy  
https://www.onsubscriber.com/iamnickpinheiro

## Downloads
https://app-provider-dev-001.azurewebsites.net/resources
- How to Build a SaaS Service on Azure (Webinar Slide Deck)  
- Multitenant SaaS Micrososervice Architecture Diagram


## License
The Azure SaaS Development Kit is licensed under the MIT license. See the LICENSE file for more details.
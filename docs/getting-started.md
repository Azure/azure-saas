To do: Write this guide. :-)

## Modules

### UX
- [Web App: Provider Web App](../src/Saas.Provider/README.md)
	- Onboarding Flow
	- Tenant Resolution Options
	- Tenant Administration for Tenant Roles (Owner, Administrator, etc.)
- Web App: SaaS Service Administration

### Compute
- App Service Plan
- Applications Insights
- Azure Kubernetes Services

### Logic
- API App: Onboarding API
- API App: Orders API
- API App: Customers API
- API App: Billing API
	- Stripe integration

### Data Access
- Entity Framework Core Data Access solution

### Data
- Azure SQL Server: Hosting off all Azure SQL Databases
- Azure SQL Server: Elastic Pooling, Elastic Jobs and Elastic Queries
- Azure SQL Database: Tenant databases supporting both Single and Multitenant scenarios
- Azure SQL Database: Catalog Database
- Azure Cosmos DB: Onboarding Flow datastore

### Communications
- API App: Push Notifications API
- Function App: Notifications API
	- SendGrid integration for Email notifications
	- Azure Communication Services integration for SMS notifications
- Logic App: New User notifications
	- SQL Connector

### Identity
- ASP.NET Core Identity
- Azure AD B2C
- Azure AD

### Storage
- Web Deployment Packages for all solutions components

### Monitoring
- Azure Application Insights included for all App Services

### Deployment
- Simplified deployment with 'Deploy to Azure' for each microservice
- Azure Resource Manager (ARM) Templates for each microservice

## Solution Roadmap
- Azure Kubernetes Services (AKS) for tenant containerization
- Azure Container Registry (ACR)
- Azure Bicep for Azure resource deployments
- Xamarin Cross Platform Mobile Apps - Multitenant
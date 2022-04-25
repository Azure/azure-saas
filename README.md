# Azure SaaS Development Kit (ASDK)

[![.NET Onboarding](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-onboarding.yml/badge.svg)](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-onboarding.yml)   [![.NET Identity](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-identity.yml/badge.svg)](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-identity.yml)   [![.NET Provider](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-provider.yml/badge.svg)](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-provider.yml)   [![.NET Admin](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-admin.yml/badge.svg)](https://github.com/Azure/azure-saas/actions/workflows/dotnet-saas-admin.yml)

The Azure SaaS Development Kit (ASDK) provides a reference architecture, deployable reference implementation and tools to help developers, startups, ISVs and Enterprises deliver their applications as a SaaS service.  A platform for platform creators.  (Public Preview)

[![Deploy to Azure](https://www.azuresaas.net/assets/images/deploy-to-azure.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json)

## Included in the Kit
1. A multitenant reference architecture
1. A sample reference implemenation that can be deployed in minutes
1. Documentation with tips, tricks, and best practices related to onboarding new tenants, elasticity, operational architecture, billing, identity, security, monitoring and more
1. Links to production SaaS platforms built using the Azure SaaS Development Kit

<!-- https://www.azuresaas.net -->

## Reference Architecture

<img src="docs/azure-saas-docs/assets/images/azure-saas-multitenant-architecture.png" width="850">

## Reference Implementation
The reference implementation provides an end-to-end SaaS service including all required microservices and their corresponding data stores to power your SaaS service.  Simply [deploy](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json) to your Azure subscription, clone the repo and migrate your business logic.

<!-- Demo SaaS Service:  https://www.azuresaas.net -->

## Deployment
[Deploy](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json) an instance of the reference implemenation in less than 5 minutes.  Once the deployment completes, you'll have all the resources deployed in your Azure subscription. Please be aware that while the costs are low, you are responsible for any charges incurred.  Deploy the full service or deploy [microservices / componenents](docs/components.md) indivually.

<img src="docs/azure-saas-docs/assets/images/azure-saas-multitenant-deployment.png" width="850">

## Production SaaS Reference

<a href="https://www.onsubscriber.com" target="_blank">Subscriber</a> is a live production SaaS solution from ISV Modern Appz built entirely on the Azure SaaS Development Kit.</p><p><a href="https://www.onsubscriber.com" target="_blank">Subscriber</a> allows users to easily build their Email and SMS lists using social login with Facebook, Google, Apple, Email and SMS.  In addition, users can add profile pictures, bios, external links and social accounts to their tenants.

<a href="https://www.onsubscriber.com" target="_blank"><img src="docs/azure-saas-docs/assets/images/azure-saas-production-service-subscriber.png" /></a>

## Solution Roadmap
- Azure Kubernetes Services (AKS) for tenant containerization
- Azure Container Registry (ACR)
- Azure Bicep for Azure resource deployments
- .NET MAUI Cross Platform Mobile Apps - Multitenant

## Subscribe for Updates
Subscribe for email notifications of updates and new features:  
<a href="https://www.onsubscriber.com/azuresaas" target="_blank">https://www.onsubscriber.com/azuresaas</a>

## Downloads
<a href="https://www.azuresaas.net/resources" target="_blank">https://www.azuresaas.net/resources</a>
- How to Build a SaaS Service on Azure (Webinar Slide Deck)  
- Multitenant SaaS Micrososervice Architecture Diagram

## Contributing
This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit
https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License
The Azure SaaS Development Kit is licensed under the MIT license. See the LICENSE file for more details.
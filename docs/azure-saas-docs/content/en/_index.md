---
type: docs
no_list: true
---
# Welcome to the Azure SaaS Dev Kit

The Azure SaaS Development Kit(ASDK) is an open-source set of pre-built modular resources to help you launch your Software-as-a-service(SaaS) offering faster:

 Standard SaaS components deployable individually or mix-and-match
* Open-source code, allowing engineers to build by example or modify/extend to be purpose-built
* Fully documented code allows for self-serve usage

Whether you're modernizing an existing application, building a new application, or migrating your application, the SaaS dev kit can help you.

## Modules & Architecture

< insert latest architecture >
[futurestate.drawio.png - Repos (azure.com)](https://dev.azure.com/azuresaas/multitenantpaas/_git/azure-saas-docs-internal?path=/Diagrams/futurestate.drawio.png)

- **B2C Authentication Service** - Provides a flexible identity solution.
- **Core App**
	- A web app where your customers to view plans and onboard to your solution.
	- Provides you with tenant administration capabilities. (modify/remove/etc.)
- **SaaS.Application** - A sample application that you can extend or replace with your own code.

## Deployment

< Deploy to Azure button >

[Deploy](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2Fazuredeploy.json/createUIDefinitionUri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Deployment%2FSaas.Deployment.Root%2FcreateUiDefinition.json) an instance of the reference implementation in less than 5 minutes. Once the deployment completes, you'll have all the resources available in your Azure subscription.

> Please be aware that you are responsible for any charges incurred.


## Product SaaS Reference

[Subscriber](https://www.onsubscriber.com/) is a live production SaaS solution built entirely on the Azure SaaS Development Kit.

It's a real production site that allows users to easily build their Email and SMS lists using social login with Facebook, Google, Apple, Email and SMS. In addition, users can add profile pictures, bios, external links and social accounts to their tenants.

## Additional Recommended Resources

* [Best practices for architecting multi-tenant solutions](https://aka.ms/multitenancy)
* [ISV Considerations for Azure landing zones](https://aka.ms/isv-landing-zones)
* [Azure Well-Architected Framework](https://docs.microsoft.com/en-us/azure/architecture/framework/)
* [WingTips Tickets SaaS Application](https://docs.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-welcome-wingtip-tickets-app) - Provides details into tradeoffs with various tenancy models within the database layer.

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit [https://cla.microsoft.com](https://cla.microsoft.com/).

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## [License](https://github.com/Azure/azure-saas#license)

The Azure SaaS Development Kit is licensed under the MIT license. See the LICENSE file for more details.
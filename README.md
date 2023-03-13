# Azure SaaS Development Kit (ASDK)

![The architecture diagram for the Azure SaaS Dev Kit (ASDK)](docs/azure-saas-docs/static/diagrams/overview.drawio.png)

## Overview

The Azure SaaS Development Kit (ASDK) is an solid starting point for building [cloud-native](https://learn.microsoft.com/en-us/dotnet/architecture/cloud-native/definition) Software as a Service (SaaS) solutions. Created for developers and architects building platforms and solutions for startups, ISVs, and enterprises, the ASDK offers a reference architecture based on best practices and design patterns.

For more information, including a Quick Start guide for deploying a running version of the ASDK, please refer to the [ASDK Documentation](https://azure.github.io/azure-saas/).

## Contents

### Modules

- [Identity Foundation Services](./src/Saas.Identity/Saas.IdentityProvider) - The core deployment and configuration of the infrastructure and services for the ASDK.
- [Admin Service](src/Saas.Admin) - Primary services administrating Tenant info and providing relevant information to frontend applications
- [Permissions Service](src/Saas.Identity/Saas.Permissions) - Service utilized by the Admin services to determine authorization by providing permissions claims to the identity provider.
- [Signup Application Web](src/Saas.SignupAdministration) - MVC web application for new Tenant signup
- [SaaS Application Web](src/Saas.Application) - Razor application providing the SaaS service to registered tenants

For each of the modules, documentation and deployment details are provided. 

### GitHub Workflows

Yaml files [have been included](.github\workflows) that define [GitHub workflow actions](https://docs.github.com/en/actions/using-workflows/about-workflows), including scripts which publish container images of your modules to the [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry). Read the corresponding [documentation](https://azure.github.io/azure-saas/resources/container-publishing/) for more information.

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit
https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repositories using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License
The Azure SaaS Development Kit is licensed under the MIT license. See the LICENSE file for more details.

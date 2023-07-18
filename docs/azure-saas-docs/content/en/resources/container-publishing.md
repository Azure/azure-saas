---
type: docs
title: "Container Publishing"
weight: 300
---

[Containers](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/) offer a high degree of platform flexibility and scalability necessary for SaaS ecosystems, and this project has been made container-ready for this reason. The [.github/workflows](https://github.com/Azure/azure-saas/tree/main/.github/workflows) directory hosts Yaml files utilized as part of [GitHub Workflows](https://docs.github.com/en/actions/using-workflows/about-workflows). Actions triggered by PR creation targeting the `main` branch handle publishing of container images to [Github Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry). These container images are then pulled by the app services running the modules (if you followed the instructions listed in our [Quick Start](https://azure.github.io/azure-saas/quick-start/) guide).

These processes are [defined by files](https://docs.github.com/en/actions/using-workflows/triggering-a-workflow) that GitHub will automatically recognize, and so comitting them to a GitHub repo will include them for use in your own project. Here are some key files and attributes to familiarize yourself with the publishing process:

## Pull-Based vs Push-Based Deployments

Using containers changes the way a traditional CI/CD process works, and moves away from a "push-based" deployment into a "pull-based" deployment. This means that in order to release code using containers, you will first need to build and publish your container (with your code and runtime inside), and then trigger your hosting environment to pull the new version of your container on your instance. See our [CI/CD for custom containers](https://docs.microsoft.com/en-us/azure/app-service/deploy-ci-cd-custom-container) documentation for a more thorough explanation of how this process works using Azure App Service. 


### i. Building Images

As part of `build-artifacts.yml` each of the modules is built as a [Docker](https://docs.docker.com/get-started/overview/) image by invoking `docker-compose.yml`. The compose script defines the overall execution parameters of each module and identifies a corresponding `Dockerfile` to define how a given module is built for use in a container.

### ii. Pushing Image

Containers are published at three times in the project's workflows:

1. **Merge**: Whenever a merge occurs on `main` then [build-artifacts.yml](https://github.com/Azure/azure-saas/blob/main/.github/workflows/build-artifacts.yml) is run. This workflow builds all of the images registered in the [docker-compose.yml](https://github.com/Azure/azure-saas/blob/main/docker-compose.yml) file. It will push each image tagged as `latest` to the GitHub Container Registry.
2. **Tag**: When a tag is created on the project then four corresponding actions are triggered which build a module and push an image tagged with the newly-created repo tag. These scripts are:
    - [container-image-build-saas-admin-tag.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-admin-tag.yml)
    - [container-image-build-saas-application-tag.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-application-tag.yml)
    - [container-image-build-saas-permissions-tag.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-permissions-tag.yml)
    - [container-image-build-saas-signupadministration-tag.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-signupadministration-tag.yml)
3. **PR**: When a PR is opened, synchronized, or reopened targeting `main`, then four corresponding actions are triggered which build a module and push an image tagged with the PR number. These scripts are:
    - [container-image-build-saas-admin-pr.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-admin-pr.yml)
    - [container-image-build-saas-application-pr.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-application-pr.yml)
    - [container-image-build-saas-permissions-pr.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-permissions-pr.yml)
    - [container-image-build-saas-signupadministration-pr.yml](https://github.com/Azure/azure-saas/blob/feature/Julian/PullBasedContainerPublishingActions/.github/workflows/container-image-build-saas-signupadministration-pr.yml)

The constructed images and identifiers are then pushed to the GitHub Container Registry service for future instances of the application modules to pull when relevant. No publishing action is necessary at this point and existing container instances will continue running their contained version without issue.

### iii. Pulling Image

Continuous Delivery has been enabled via the Tag and PR workflows to enable testing and dev environment automatic update. These scripts are configured to fetch a webhook url secret from the repository corresponding to an environment and the given module. It will then issue an `HTTP POST` against this endpoint so that it can be alerted that a new image version has become available. The endpoint in use with the existing repository is the provided CI/CD webhook for the Azure Web App [custom container image](https://docs.microsoft.com/en-us/azure/app-service/deploy-ci-cd-custom-container?tabs=private&pivots=container-linux#4-enable-cicd) endpoint.

## Scripts and Variables

### i. Build Artifacts
The [build-artifacts.yml](https://github.com/Azure/azure-saas/blob/main/.github/workflows/build-artifacts.yml) script is invoked whenever there is a PR issued against the `main` branch. It defines two steps for building and publishing the module Docker images.

| Variable       | Description                                                                                                | Default                                                    |
| -------------- | ---------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| IMAGES_TO_PUSH | Quoted and space-delimited list of images to push.                                                         | ("asdk-admin" "asdk-web" "asdk-permissions" "asdk-signup") |
| GH_TAG         | Tag applied to image to identify it and indicate its version. Currently version is always set to `latest`. |                                                            |

### ii. Docker Compose
The `docker-compose build` command in the build artifacts script finds and invokes the [Docker Compose specification](https://docs.docker.com/compose/compose-file/) script [docker-compose.yml](https://github.com/Azure/azure-saas/blob/main/docker-compose.yml) in the project root. This lists out the images to construct and indicates a corresponding `Dockerfile` for an image.

| Variable        | Section | Description                                                                                                                                | Default |
| --------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------ | ------- |
| image           |         | The image identifier, must match with the image name variable in `build-artifacts.yaml` to be tagged and pushed into container repository. |         |
| container_name  |         | Container identifier, distinct from the image it is started from.                                                                          |         |
| context         | build   | Defines the directory context from project root other relative paths are searched against                                                  | ./src   |
| dockerfile      | build   | Path relative to context to the Dockerfile for a given image specification                                                                 |         |
| expose          |         | Defines the ports that must be exposed from container.                                                                                     | 80      |
| ports           |         | Container ports to expose for access.                                                                                                      |         |

### iii. Dockerfile
Each module directory contains a corresponding [Dockerfile](https://docs.docker.com/engine/reference/builder/) referenced by `docker-commpose.yml` which contains the commands necessary to construct its container image.
- [Admin Service](https://github.com/Azure/azure-saas/blob/main/src/Saas.Admin/Saas.Admin.Service/Dockerfile)
- [Permissions Service](https://github.com/Azure/azure-saas/blob/main/src/Saas.Identity/Saas.Permissions/Saas.Permissions.Service/Dockerfile)
- [Signup Application Web](https://github.com/Azure/azure-saas/blob/main/src/Saas.SignupAdministration/Saas.SignupAdministration.Web/Dockerfile)
- [SaaS Application Web](https://github.com/Azure/azure-saas/blob/main/src/Saas.Application/Saas.Application.Web/Dockerfile)

| Variable    | Section | Description                                                                    | Default |
| ----------- | ------- | ------------------------------------------------------------------------------ | ------- |
| app_name    | Shared  | Informal identifier                                                            |         |
| app_path    | Shared  | Path to project directory from context defined in compose specification.       |         |
| dll_name    | Stage 2 | Name of the dll in output directory                                            |         |
| ENTRYPOINT  | Stage 2 | Command which directs the container to the path and dll as entry to the image. |         |

### iv. GitHub Action Secrets

The following secrets are referenced by the GitHub Actions CI/CD workflows to issue a POST alerting deployed apps of newly-available container images.

| Variable                         | Environment | Module                         |
| -------------------------------- | ----------- | ------------------------------ |
| ASDK_ADMIN_DEV_WEBHOOK_URL       | Dev         | Saas.Admin.Service             |
| ASDK_WEB_DEV_WEBHOOK_URL         | Dev         | Saas.Application.Web           |
| ASDK_PERMISSIONS_DEV_WEBHOOK_URL | Dev         | Saas.Permissions.Service       |
| ASDK_SIGNUP_DEV_WEBHOOK_URL      | Dev         | Saas.SignupAdministration.Web  |




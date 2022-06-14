---
type: docs
title: "Container Publishing"
weight: 300
---

[Containers](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/) offer a high degree of platform flexibility and scalability necessary for SaaS ecosystems, and this project has been made container-ready for this reason. The [.github/workflows](https://github.com/Azure/azure-saas/tree/main/.github/workflows) directory hosts Yaml files utilized as part of [GitHub Workflows](https://docs.github.com/en/actions/using-workflows/about-workflows). Actions triggered by PR creation targeting the `main` branch handle publishing of container images to [Github Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry). These container images are then pulled by the app services running the modules (if you followed the instructions listed in our [Quick Start](https://azure.github.io/azure-saas/quick-start/) guide).

These processes are [defined by scripts](https://docs.github.com/en/actions/using-workflows/triggering-a-workflow) GitHub will automatically recognize, and so comitting them to a GitHub repo will include them for use in your own project. Here are some key files and attributes to familiarize yourself with the publishing process:

## Pull-Based vs Push-Based Deployments

Using containers changes the way a traditional CI/CD process works, and moves away from a "push-based" deployment into a "pull-based" deployment. This means that in order to release code using containers, you will first need to build and publish your container (with your code and runtime inside), and then trigger your hosting environment to pull the new version of your container on your instance. See our [CI/CD for custom containers](https://docs.microsoft.com/en-us/azure/app-service/deploy-ci-cd-custom-container) documentation for a more thorough explanation of how this process works using Azure App Service. 


### i. Building Images

As part of `build-artifacts.yml` each of the modules is built as a [Docker](https://docs.docker.com/get-started/overview/) image by invoking `docker-compose.yml`. The compose script defines the overall execution parameters of each module and identifies a corresponding `Dockerfile` to define how a given module is built for use in a container.

### ii. Pushing Image

 The necessary identifiers have been included so that each new version will supplant all others as `latest`. The constructed images and identifiers are then pushed to the GitHub Container Registry service for future instances of the application modules to pull when relevant. No publishing action is necessary at this point and existing container instances will continue running their contained version without issue.

### iii. Pulling Image

Active containers can be prompted to update by [posting to a webhook created in the Docker Compose file](https://docs.microsoft.com/en-us/azure/app-service/deploy-ci-cd-custom-container?tabs=private&pivots=container-linux#4-enable-cicd) when the relevant module image is published, at which point they can pull the latest image when appropriate.

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
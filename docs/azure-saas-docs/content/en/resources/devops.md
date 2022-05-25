---
type: docs
title: "DevOps Workflows"
weight: 100
toc_hide: true
---

For your convenience, we have provided some sample GitHub workflows that you can use to build and deploy code changes to each module. These workflows are the same ones that we use to package and release the code to our test environment. They were created as a baseline reference with the intent to be extensible when needed.

## How does it work?
There are 2 [reusable workflows](https://docs.github.com/en/actions/using-workflows/reusing-workflows) we have created which the process is based on.

Each module has 2 workflow files that implement these re-usable workflows ensuring each build and deploy step taken is consistent across the entire solution. Each module workflow file is responsible for defining when the workflow is triggered, setting up variables, and calling the correct reusable workflow.

### Process Outline
### **Build & Deploy to Staging**
Workflow File: `template-pr-deploy.yml`

- `pr-deploy-saas-admin.yml`
- `pr-deploy-saas-application.yml`
- `pr-deploy-saas-permissions.yml`
- `pr-deploy-saas-signupadministration.yml`

Triggered On: `Pull Request targeting main branch`

#### Input Variables

| Input Name                      | Description                                                                                                                          | Default                                  |
|---------------------------------|--------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------|
| dotnet_version                  | The version of the .NET project to build                                                                                             |                                          |
| artifact_name                   | The name of the artifact file produced                                                                                               |                                          |
| app_service_name                | The name of the app service to deploy the artifact to                                                                                |                                          |
| app_service_resource_group_name | The name of the resource group the app service resides in                                                                            |                                          |
| project_build_path              | The path of the folder that the .csproj for the module is in                                                                         |                                          |
| slot_name                       | The name of the deployment slot  to create and deploy to on the app service (Used for override, recommended to keep the default) | pr-${{github.event.pull_request.number}} |

#### Secrets

- `AZURE_CREDENTIALS`
  - The CI/CD pipeline must contain Azure credentials for a service principal that has an appropriate access level to create a new Azure App Service slot and deploy to it. See [this](https://docs.microsoft.com/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows) document for setting up these credentials in your own repo. 

#### Job Breakdown
There are 3 jobs within this workflow: `build`, `create-deployment-slot`, and `deploy-to-slot`. Here is a high level overview of what each job does.

1. `build`
   1. Run the .NET restore, build, and publish commands on the project. Project is determined by the `project_build_path` input variable.
   2. Publishes built .NET artifact to GitHub artifacts. Artifact is named with the string provided in the `artifact_name` input variable.

2. `create-deployment-slot`
   1. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
   2. Runs Azure CLI command to create a new deployment slot in the Azure App Service provided in the `app_service_name` input variable. Slot name is provided via the `slot_name` input variable.

3. `deploy-to-slot`
Depends on the `build` and `create-deployment-slot` jobs to succeed in order to run. 
   1. Downloads the build artifact from the `build` job. Downloaded artifact name is provided via the `artifact_name` input variable.
   2. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
   3. Initiates deployment of the downloaded artifact to the app service and slot specified in `app_service_name` and `slot_name` respectively using the [azure/webapps-deploy](https://github.com/Azure/webapps-deploy) GitHub Action.

### **Swap Staging Slot into Production**

Workflow File: `template-pr-merge.yml`

Used By:

- `pr-merge-saas-admin.yml`
- `pr-merge-saas-application.yml`
- `pr-merge-saas-permissions.yml`
- `pr-merge-saas-signupadministration.yml`

Triggered On: `Pull request close`

#### Input Variables

| Input Name                      | Description                                                                                                         | Default                                  |
|---------------------------------|---------------------------------------------------------------------------------------------------------------------|------------------------------------------|
| app_service_name                | The name of the app service the slot is deployed to                                                                 |                                          |
| app_service_resource_group_name | The name of the resource group the app service resides in                                                           |                                          |
| slot_name                       | The name of the deployment slot the staged code is deployed to (Used for override, recommended to keep the default) | pr-${{github.event.pull_request.number}} |

#### Secrets

- `AZURE_CREDENTIALS`
  - The CI/CD pipeline must contain Azure credentials for a service principal that has an appropriate access level to perform the swap operation on the Azure App Service in question. See [this](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows) document for setting up these credentials in your own repo. 

#### Job Breakdown

There are 2 main jobs within this workflow: `swap-slot` and `delete-slot`

1. `swap-slot` - *This job only runs if the PR is closed AND merged. This job does not run if the PR is closed without merging.*
   1. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
   2. Runs the Azure CLI command to swap the deployment slot named in the `slot_name` input variable with the production slot on the Azure App Service named in the `app_service_name` input variable. 

2. `delete-slot` - This job will only run if the preceding step runs and succeeds. If the PR is closed without merging, the previous step will be skipped but this will still run.
   1. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
   2. Runs the Azure CLI command to delete the deployment slot named in the `slot_name` input variable with the production slot on the Azure App Service named in the `app_service_name` input variable. 

> **Important**: You may choose to not delete the deployment slot directly following a deployment to retain the ability to swap the slot back if there are any issues and you'd like to undo the deployment. Deleting the slot immediately after a deployment is most reccomended in a development/test environment where you may be deploying multiple times per day.

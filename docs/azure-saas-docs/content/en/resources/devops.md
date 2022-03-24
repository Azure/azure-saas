---
type: docs
title: "DevOps Workflows"
weight: 100
---

For your convenience, we have provided some sample GitHub workflows that you can use to build and deploy code changes to each module. They were created as a baseline reference with the intent to be extensible when needed.


## How does it work?
There are 2 [reusable workflows](https://docs.github.com/en/actions/using-workflows/reusing-workflows) we have created which the process is based on. 

Each module implements these re-usable workflows ensuring each build and deploy step taken is consistent across the entire solution. There are 2 github workflow files per module. Each module workflow file is responsible for defining when the workflow is triggered, setting up variables, and calling the correct reusable workflow.



### Process Outline
#### **Build & Deploy to Staging**
Workflow File: `template-pr-deploy.yml`

Triggered On: `Pull Request targeting main branch`

Input Variables:

There are 3 jobs within this workflow: `build`, `create-deployment-slot`, and `deploy-to-slot`. Here is a high level overview of what each job does. 

##### `build`
1. Run the .NET restore, build, and publish commands on the project. Project is determined by the `project_build_path` input variable.
2. Publishes built .NET artifact to GitHub artifacts. Artifact is named with the string provided in the `artifact_name` input variable.

##### `create-deployment-slot`
1. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
2. Runs Azure CLI command to create a new deployment slot in the app service provided in the `app_service_name` input variable. Slot name is provided via the `slot_name` input variable.

##### `deploy-to-slot`
Depends on the `build` and `create-deployment-slot` jobs to succeed in order to run. 
1. Downloads the build artifact from the `build` job. Downloaded artifact name is provided via the `artifact_name` input variable.
2. Logs into the Azure CLI with [credentials provided](https://docs.microsoft.com/en-us/azure/developer/github/connect-from-azure?tabs=azure-portal%2Cwindows#create-a-service-principal-and-add-it-as-a-github-secret) in the `AZURE_CREDENTIALS` secret.
3. Initiates deployment of the downloaded artifact to the app service and slot specified in `app_service_name` and `slot_name` respectively using the [azure/webapps-deploy](https://github.com/Azure/webapps-deploy) GitHub Action.

### 
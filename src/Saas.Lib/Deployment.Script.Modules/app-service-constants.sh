#!/usr/bin/env bash

# disable unused variable warning https://www.shellcheck.net/wiki/SC2034
# shellcheck disable=SC2034

# configuration manifest for the Identity Foundation deployment, run previously
CONFIG_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config"
CONFIG_FILE="${CONFIG_DIR}/config.json"

# local bicep script directory
BICEP_DIR="${BASE_DIR}/bicep"
BICEP_PARAMETERS_DIR="${BICEP_DIR}/Parameters"
BICEP_APP_SERVICE_DEPLOY_PARAMETERS_FILE="${BICEP_PARAMETERS_DIR}/app-service-parameters.json"
BICEP_CONFIG_ENTRIES_DEPLOY_PARAMETERS_FILE="${BICEP_PARAMETERS_DIR}/config-entries-parameters.json"
BICEP_PARAMETERS_TEMPLATE_FILE="${BICEP_PARAMETERS_DIR}/parameters-template.json"
BICEP_IDENTITY_FOUNDATION_OUTPUT_FILE="${BICEP_PARAMETERS_DIR}/identity-foundation-outputs.json"

DEPLOY_APP_SERVICE_BICEP_FILE="${BICEP_DIR}/deployAppService.bicep"
DEPLOY_CONFIG_ENTRIES_BICEP_FILE="${BICEP_DIR}/deployConfigEntries.bicep"

DEPLOYMENT_CONTAINER_NAME="asdk-script-deployment:latest"

ACT_CONTAINER_DIR="${REPO_BASE}/src/Saas.Lib/Act.Container"

# act container name
ACT_CONTAINER_NAME="act-container:latest"

# secrets file
ACT_SECRETS_DIR="/asdk/act/.secret"
ACT_SECRETS_FILE="${ACT_SECRETS_DIR}/secret"
ACT_SECRETS_FILE_RG="${ACT_SECRETS_DIR}/resource_group.txt"

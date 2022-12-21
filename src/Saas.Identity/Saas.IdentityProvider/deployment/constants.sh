#!/usr/bin/env bash

# user directories
BASE_AZURE_CONFIG_DIR="$HOME/.azure"
B2C_USR_AZURE_CONFIG_DIR="${HOME}/b2c/.azure"
SP_USR_AZURE_CONFIG_DIR="${HOME}/sp/.azure"

if [[ -z $ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR ]] ; then
    echo "ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR is not set. Please run 'start.sh' from the root of the project using start.sh."
    echo "Or use command 'export ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR=\$PWD' if you are in the root of the project directory - i.e. .../deployment."
    exit 1
fi

# script directories
BASE_DIR="${ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR}"
SCRIPT_DIR="${BASE_DIR}/script"
SCRIPT_MODULE_DIR="${SCRIPT_DIR}/module"
CONFIG_DIR="${BASE_DIR}/config"
BICEP_DIR="${BASE_DIR}/bicep"

CONFIG_FILE="${CONFIG_DIR}/config.json"
CONFIG_TEMPLATE_FILE="${CONFIG_DIR}/config-template.json"

IDENTITY_BICEP_PARAMETERS_FILE="${CONFIG_DIR}/identity-bicep-parameters.json"
IDENTITY_BICEP_PARAMETERS_TEMPLATE_FILE="${CONFIG_DIR}/identity-bicep-parameters-template.json"

MAIN_BICEP_PARAMETERS_FILE="${CONFIG_DIR}/main-deployment-parameters.json"
MAIN_BICEP_PARAMETERS_TEMPLATE_FILE="${CONFIG_DIR}/main-deployment-parameters-template.json"

IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE="${BASE_DIR}/../policies/appsettings.json"
IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_DIR="${BASE_DIR}/../policies/"
IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR="${BASE_DIR}/../policies/Environments"

MAIN_IDENTITY_DEPLOY_FILE="${BICEP_DIR}/IaC/main-identity-deploy.bicep"

# other directories and files
LOG_FILE_DIR="${BASE_DIR}/log"

CERTIFICATE_POLICY_FILE="${CONFIG_DIR}/certificate-policy.json"

SECRET_DIR_NAME="secretDir"
CERTIFICATE_DIR_NAME="certificateDir"

# Microsoft Graph api id
GRAPH_APP_ID="00000003-0000-0000-c000-000000000000"
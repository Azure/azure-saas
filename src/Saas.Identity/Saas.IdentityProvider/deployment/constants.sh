#!/usr/bin/env bash

# disable unused variable warning https://www.shellcheck.net/wiki/SC2034
# shellcheck disable=SC2034

# user directories
BASE_AZURE_CONFIG_DIR="$HOME/.azure"
B2C_USR_AZURE_CONFIG_DIR="${HOME}/b2c/.azure"
SP_USR_AZURE_CONFIG_DIR="${HOME}/sp/.azure"

# repo base
repo_base="$( git rev-parse --show-toplevel )"
REPO_BASE="${repo_base}"

# name of the deployment script
SCRIPT_NAME="Identity Foundation Deployment"

# base directory and script directory for the Identity Foundation deployment:
BASE_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment"

# global script directory
SHARED_MODULE_DIR="${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules"

# local script directory
SCRIPT_DIR="${BASE_DIR}/script"

# configuration files, log and bicep directories specific 
# for the Identity Foundation deployment:
CONFIG_DIR="${BASE_DIR}/config"
LOG_FILE_DIR="${BASE_DIR}/log"
BICEP_DIR="${BASE_DIR}/bicep"
CONFIG_FILE="${CONFIG_DIR}/config.json"
CONFIG_TEMPLATE_FILE="${CONFIG_DIR}/config-template.json"

IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE="${CONFIG_DIR}/identity-bicep-parameters.json"
IDENTITY_FOUNDATION_BICEP_PARAMETERS_TEMPLATE_FILE="${CONFIG_DIR}/identity-bicep-parameters-template.json"

IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/policies/appsettings.json"
IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/policies/"
IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/policies/Environments"

DEPLOY_IDENTITY_FOUNDATION_FILE="${BICEP_DIR}/IdentityFoundation/deployIdentityFoundation.bicep"

CERTIFICATE_POLICY_FILE="${CONFIG_DIR}/certificate-policy.json"

SECRET_DIR_NAME="secretDir"
CERTIFICATE_DIR_NAME="certificateDir"

# Microsoft Graph api id
GRAPH_APP_ID="00000003-0000-0000-c000-000000000000"
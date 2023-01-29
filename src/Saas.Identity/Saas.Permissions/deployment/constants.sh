#!/usr/bin/env bash

# disable unused variable warning https://www.shellcheck.net/wiki/SC2034
# shellcheck disable=SC2034

# repo base
repo_base="$( git rev-parse --show-toplevel )"
REPO_BASE="${repo_base}"

# project base directory
BASE_DIR="${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment"

# global script directory
SHARED_MODULE_DIR="${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules"

# local script directory
SCRIPT_DIR="${BASE_DIR}/script"

#local log directory
#LOG_FILE_DIR="${BASE_DIR}/log"

# configuration manifest for the Identity Foundation deployment, run previously
CONFIG_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config"
CONFIG_FILE="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/config.json"

# act directory
ACT_DIR="${BASE_DIR}/act"

# act container name
ACT_CONTAINER_NAME="act-container:latest"

WORKFLOW_BASE="${REPO_BASE}/.github/workflows"
PERMISSIONS_DEPLOYMENT_WORKFLOW_FILE="${WORKFLOW_BASE}/permissions-api-deploy.yml"
ACT_PERMISSIONS_DEPLOYMENT_LOCAL_WORKFLOW_FILE="${ACT_DIR}/workflows/permissions-api-deploy-debug.yml"

# secrets file
ACT_SECRETS_DIR="/asdk/act/.secret"
ACT_SECRETS_FILE="/asdk/act/.secret/secret"
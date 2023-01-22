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

# script directories
BASE_DIR="${ASDK_PERMISSIONS_API_DEPLOYMENT_BASE_DIR}"

# global script directory
SHARED_MODULE_DIR="${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules"

# local script directory
SCRIPT_DIR="${BASE_DIR}/script"

#local log directory
LOG_FILE_DIR="${BASE_DIR}/log"

# configuration manifest for the Identity Foundation deployment, run previously
CONFIG_DIR="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config"
CONFIG_FILE="${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/config.json"
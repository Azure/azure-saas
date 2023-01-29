#!/usr/bin/env bash

# disable unused variable warning https://www.shellcheck.net/wiki/SC2034
# shellcheck disable=SC2034

# app naming
APP_NAME="permissions-api"
APP_DEPLOYMENT_NAME="permissionApi"

# repo base
repo_base="$(git rev-parse --show-toplevel)"
REPO_BASE="${repo_base}"

# project base directory
BASE_DIR="${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment"

# local script directory
SCRIPT_DIR="${BASE_DIR}/script"

#local log directory
LOG_FILE_DIR="${BASE_DIR}/log"

# act directory
ACT_DIR="${BASE_DIR}/act"

# GitHub workflows
WORKFLOW_BASE="${REPO_BASE}/.github/workflows"
GITHUB_ACTION_WORKFLOW_FILE="${WORKFLOW_BASE}/permissions-api-deploy.yml"
ACT_LOCAL_WORKFLOW_DEBUG_FILE="${ACT_DIR}/workflows/permissions-api-deploy-debug.yml"

# global script directory
SHARED_MODULE_DIR="${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules"

# adding app service global constants
source "${SHARED_MODULE_DIR}/app-service-constants.sh"

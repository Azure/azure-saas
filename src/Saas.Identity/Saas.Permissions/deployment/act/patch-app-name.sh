#!/usr/bin/env bash

# repo base
repo_base="$( git rev-parse --show-toplevel )"
base_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"

# shellcheck disable=SC1091
source "${base_dir}/constants.sh"

echo "Setting up SaaS Permissions Service API deployment web app name."
"${SCRIPT_DIR}/patch-workflow.py" "${CONFIG_FILE}" "${ACT_PERMISSIONS_DEPLOYMENT_LOCAL_WORKFLOW_FILE}"


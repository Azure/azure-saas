#!/usr/bin/env bash

# if not running in a container
if ! [ -f /.dockerenv ]; then
    echo "Running outside of a container us not supported. Please run the script using './run.sh'."
    exit 0
fi

# repo base
repo_base="$( git rev-parse --show-toplevel )"
base_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"

# shellcheck disable=SC1091
{
    source "${base_dir}/constants.sh"
}

if ! [[ -f $CONFIG_FILE ]]; then
    echo "The ASDK Identity Foundation has not completed. Please run the Identity Foundation deployment script first."
    exit 0
fi

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

echo "Setting up SaaS Permissions Service API deployment web app name."
"${SCRIPT_DIR}/patch-workflow.py" "${CONFIG_FILE}" "${PERMISSIONS_DEPLOYMENT_WORKFLOW_FILE}"
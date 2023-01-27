#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
{
    source "./constants.sh"
}

if [[ -f $CONFIG_FILE ]]; then
    echo "The ASDK Identity Foundation has not completed. Please run the Identity Foundation deployment script first."
    exit 0
fi

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

echo "Setting up SaaS Permissions Service API deployment web app name."
"${SCRIPT_DIR}/patch-workflow.py" "${CONFIG_FILE}" "${PERMISSIONS_DEPLOYMENT_WORKFLOW_FILE}"
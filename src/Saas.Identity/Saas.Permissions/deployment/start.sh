#!/usr/bin/env bash

export ASDK_CACHE_AZ_CLI_SESSIONS=true

if [[ -z $ASDK_PERMISSIONS_API_DEPLOYMENT_BASE_DIR ]]; then
    # repo base
    echo "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE is not set. Setting it to default value."
    repo_base="$( git rev-parse --show-toplevel )"
    project_base="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"
    export ASDK_PERMISSIONS_API_DEPLOYMENT_BASE_DIR="${project_base}"
fi

if [[ -f $CONFIG_FILE ]]; then
    echo "The ASDK Identity Foundation has not completed. Please run the Identity Foundation deployment script first."
    exit 0
fi

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

# shellcheck disable=SC1091
# include script modules into current shell
{
    source "$ASDK_PERMISSIONS_API_DEPLOYMENT_BASE_DIR/constants.sh"
    source "$SCRIPT_DIR/init-module.sh"
}


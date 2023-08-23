#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    source "$ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
    source "$SHARED_MODULE_DIR/act-credentials-module.sh"
}

"${SHARED_MODULE_DIR}/patch-github-workflow.py" "${APP_NAME}" "${CONFIG_FILE}" "${ACT_LOCAL_WORKFLOW_DEBUG_FILE}"

# using the az cli settings and cache from the host machine
initialize-az-cli "$HOME/.azure"

resource_group_name="$(get-value ".deployment.resourceGroup.name")"

setup-act-secret "${ACT_SECRETS_FILE}" $"${resource_group_name}"

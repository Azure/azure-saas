#!/usr/bin/env bash

set -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
}

if ! [[ -f "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" ]]; then
    echo "The file ${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE} does not exist, creating it now" \
        | log-output \
            --level info
    cp "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_TEMPLATE_FILE}" "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}"
fi

set -u

"${SCRIPT_DIR}/map-identity-paramenters.py" "${CONFIG_FILE}" "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" \
    | log-output \
        --level info \
        --header "Generating Identity Foundation services parameters..." \
    || echo "Failed to map Identity Foundation services parameters" \
        | log-output \
            --level error \
            --header "Critical Error" \
            || exit 1

resource_group="$( get-value ".deployment.resourceGroup.name" )"

echo "Provisioning Identity Foundation services in resource group ${resource_group}..." \
    | log-output \
        --level info

az deployment group create \
    --resource-group "${resource_group}" \
    --name "IdentityBicepDeployment" \
    --template-file "${DEPLOY_IDENTITY_FOUNDATION_FILE}" \
    --parameters "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" \
    | log-output \
        --level info \
    || echo "Failed to deploy Identity Foundation services" \
        | log-output \
            --level error \
            --header "Critical Error" \
            || exit 1

echo "Indentity Foundation services successfully provisioned in resource group ${resource_group}..." \
    | log-output \
        --level success
#!/usr/bin/env bash

set -e -o pipefail

# include script modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/init-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"

if ! [[ -f "${IDENTITY_BICEP_PARAMETERS_FILE}" ]]; then
    echo "The file ${IDENTITY_BICEP_PARAMETERS_FILE} does not exist, creating it now" \
        | log-output \
            --level info
    cp "${IDENTITY_BICEP_PARAMETERS_TEMPLATE_FILE}" "${IDENTITY_BICEP_PARAMETERS_FILE}"
fi

set -u

"${SCRIPT_MODULE_DIR}/map-identity-paramenters.py" "${CONFIG_FILE}" "${IDENTITY_BICEP_PARAMETERS_FILE}" \
    | log-output \
        --level info \
        --header "Generating Identity Provider parameters..." \
    || echo "Failed to map Identity Provider parameters" \
        | log-output \
            --level error \
            --header "Critical Error" \
            || exit 1

resource_group="$( get-value ".deployment.resourceGroup.name" )"

echo "Provisioning permission provider in resource group ${resource_group}..." \
    | log-output \
        --level info

output="$( az deployment group create \
    --resource-group "${resource_group}" \
    --name "IdentityBicepDeployment" \
    --template-file "${MAIN_IDENTITY_DEPLOY_FILE}" \
    --parameters "${IDENTITY_BICEP_PARAMETERS_FILE}" )" \
    || echo "Failed to deploy Identity Provider" \
        | log-output \
            --level error \
            --header "Critical Error" \
            || exit 1
        
echo "${output}" \
    | log-output \
        --level info

echo "Permission provider successfully provisioned in resource group ${resource_group}..." \
    | log-output \
        --level success
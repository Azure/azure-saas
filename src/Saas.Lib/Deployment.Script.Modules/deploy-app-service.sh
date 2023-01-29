#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/deploy-service-module.sh"
}

prepare-parameters-file "${BICEP_PARAMETERS_TEMPLATE_FILE}" "${BICEP_APP_SERVICE_DEPLOY_PARAMETERS_FILE}"

resource_group="$(get-value ".deployment.resourceGroup.name")"
identity_foundation_deployment_name="$(get-value ".deployment.identityFoundation.name")"

echo "Downloading Identity Foundation outputs from Resource Group '${resource_group}' deployment named '${identity_foundation_deployment_name}'..." |
    log-output \
        --level info

get-identity-foundation-deployment-outputs \
    "${resource_group}" \
    "${identity_foundation_deployment_name}" \
    "${BICEP_IDENTITY_FOUNDATION_OUTPUT_FILE}"

echo "Mapping '${APP_NAME}' parameters..." |
    log-output \
        --level info

# map parameters
"${SHARED_MODULE_DIR}/map-output-parameters-for-app-service.py" \
    "${APP_NAME}" \
    "${BICEP_IDENTITY_FOUNDATION_OUTPUT_FILE}" \
    "${BICEP_APP_SERVICE_DEPLOY_PARAMETERS_FILE}" \
    "${CONFIG_FILE}" |
    log-output \
        --level info ||
    echo "Failed to map ${APP_NAME} services parameters" |
    log-output \
        --level error \
        --header "Critical Error" ||
    exit 1

deployment_name="$(get-value ".deployment.${APP_DEPLOYMENT_NAME}.name")"

echo "Provisioning '${deployment_name}' to resource group ${resource_group}..." |
    log-output \
        --level info

deploy-service \
    "${resource_group}" \
    "${BICEP_APP_SERVICE_DEPLOY_PARAMETERS_FILE}" \
    "${DEPLOY_APP_SERVICE_BICEP_FILE}" \
    "${deployment_name}"

echo "Done. '${deployment_name}' was successfully provisioned to resource group ${resource_group}..." |
    log-output \
        --level success

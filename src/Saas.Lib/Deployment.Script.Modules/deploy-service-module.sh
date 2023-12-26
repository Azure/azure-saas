#!/usr/bin/env bash

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "$SHARED_MODULE_DIR/log-module.sh"
}

function deploy-service() {
    local resource_group="$1"
    local parameters_file="$2"
    local template_file="$3"
    local deployment_name="$4"

    az deployment group create \
        --resource-group "${resource_group}" \
        --name "${deployment_name}" \
        --template-file "${template_file}" \
        --parameters "${parameters_file}" \
        || echo "Failed to deploy to ${APP_NAME}. This sometimes happens, please try again." \
            | log-output \
                --level error \
                --header "Critical Error" \
                || exit 1
}

function get-identity-foundation-deployment-outputs() {
    local resource_group="$1"
    local deployment_name="$2"
    local output_file="$3"
    local subscriptionId="$4"

    az account set --subscription "${subscriptionId}" \
        || echo "Failed to set subscription to ${subscriptionId}" \
            | log-output \
                --level error \
                --header "Critical Error" \
                || exit 1

    deployment_output_parameters="$( az deployment group show \
        --name "${deployment_name}" \
        --resource-group "${resource_group}" \
        --query properties.outputs )" \
        || echo "Failed to get Identity Bicep deployment output parameters" \
            | log-output \
                --level error \
                --header "Critical Error" \
                || exit 1
    
    echo "${deployment_output_parameters}" > "${output_file}"
}

function prepare-parameters-file() {
    local template_file="$1"
    local parameters_file="$2"

    if ! [[ -f "${parameters_file}" ]]; then
        echo "The file ${parameters_file} does not exist, creating it now" \
            | log-output \
                --level info

        cp "${template_file}" "${parameters_file}"
    fi
}

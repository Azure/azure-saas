#!/usr/bin/env bash

# shellcheck disable=SC1091
{
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
}

function is_resource_group() {
    local resource_group="${1}"

    existing_rg="$(cat "${ACT_SECRETS_FILE_RG}")" ||
        {
            false
            return
        }

    if [[ "${existing_rg}" == "${resource_group}" ]]; then
        true
    else
        false
    fi

    return
}

function setup-act-secret() {
    local act_secret_file="${1}"
    local resource_group="${2}"

    declare secret

    # Creating a new secret for accessing the OIDC app access deployment
    # if no secret exists
    # or the secret file is empty
    # or it was created for another resource group
    if [[ ! -f "${act_secret_file}" ||
        ! -s "${act_secret_file}" ]] ||
        ! is_resource_group "${resource_group}"; then
        oidc_app_name="$(get-value ".oidc.name")"
        oidc_app_id="$(get-value ".oidc.appId")"

        echo "Adding secret for the OIDC app '${oidc_app_name}'"

        touch "${act_secret_file}"

        # getting app crentials
        secret_json="$(az ad app credential reset \
            --id "${oidc_app_id}" \
            --display-name "secret-${oidc_app_name}" \
            --query "{clientId:appId, clientSecret:password, tenantId:tenant}" \
            2>/dev/null)"

        subscription_id="$(get-value ".initConfig.subscriptionId")"

        # adding subscription id to secret json
        secret_json="$(jq --arg sub_id "${subscription_id}" \
            '. | .subscriptionId = $sub_id' \
            <<<"${secret_json}")"

        # outputting secret json to file in compat format with escape double quotes
        secret="$(echo "${secret_json}" |
            jq --compact-output '.' |
            sed 's/"/\\"/g')"

        secret="AZURE_CREDENTIALS=\"${secret}\""

        echo "${secret}" >"${act_secret_file}"

        echo "${resource_group}" >"${ACT_SECRETS_FILE_RG}"

        echo "Waiting for 30 seconds to allow new secret to propagate."
        sleep 30
    else
        echo "Using existing secrets file at: ${act_secret_file}'. If you want to create a new one, please run '/.clean.sh' again and then run './run.sh' again."
    fi
}

function delete-secret-based-credentials() {
    oidc_app_id="$(get-value ".oidc.appId")"
    oidc_app_name="$(get-value ".oidc.name")"
    oidc_app_name="secret-${oidc_app_name}"

    echo "Deleting the secret based credentials for the OIDC app '${oidc_app_name}'..."

    key_id="$(az ad app credential list \
        --id "${oidc_app_id}" \
        --query "[?displayName=='${oidc_app_name}'].keyId | [0]" \
        --output tsv)"

    if [[ -n $key_id ]]; then
        az ad app credential delete \
            --id "${oidc_app_id}" \
            --key-id "${key_id}"

        echo "Secret based credentials deleted."
    else
        echo "No secret based credentials found."
    fi
}

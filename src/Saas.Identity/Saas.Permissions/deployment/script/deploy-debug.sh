#!/usr/bin/env bash

# repo base
repo_base="$( git rev-parse --show-toplevel )"
base_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"

# shellcheck disable=SC1091
{
    source "$base_dir/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
}

"${SCRIPT_DIR}"/patch-app-name.sh

# using the az cli settings and cache from the host machine
initialize-az-cli "$HOME/.azure"

declare secret

# Creating a new secret for accessing the OIDC app access deployment.
if  [[ ! -f "${ACT_SECRETS_FILE}" || ! -s "${ACT_SECRETS_FILE}" ]]; then
     oidc_app_name="$( get-value ".oidc.name" )"
     oidc_app_id="$( get-value ".oidc.appId" )"

    echo "Creating the OIDC app '${oidc_app_name}'"

    touch "${ACT_SECRETS_FILE}"
    
    # getting app crentials
    secret_json="$( az ad app credential reset \
        --id "${oidc_app_id}" \
        --display-name "secret-${oidc_app_name}" \
        --query "{clientId:appId, clientSecret:password, tenantId:tenant}" \
        2> /dev/null )"

    subscription_id="$( get-value ".initConfig.subscriptionId" )"

    # adding subscription id to secret json
    secret_json="$( jq --arg sub_id "${subscription_id}" \
        '. | .subscriptionId = $sub_id' \
        <<< "${secret_json}" )"

    # outputting secret json to file in compat format with escape double quotes 
    secret="$( echo "${secret_json}" \
        | jq --compact-output '.' \
        | sed 's/"/\\"/g' )"

    secret="AZURE_CREDENTIALS=\"${secret}\"" 

    echo "${secret}" > "${ACT_SECRETS_FILE}"

    echo "Waiting for 30 seconds to allow new secret to propagate."
    sleep 30
else
    echo "Using existing secrets file at: '${ACT_SECRETS_FILE}'. If you want to create a new one, please run '/.clean.sh' again and then run './run.sh'."
fi
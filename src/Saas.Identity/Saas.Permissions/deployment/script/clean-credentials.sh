#!/usr/bin/env bash

# repo base
repo_base="$( git rev-parse --show-toplevel )"
base_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"

# shellcheck disable=SC1091
{
    source "${base_dir}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
}

# initialize az cli
initialize-az-cli "$HOME/.azure"

sudo rm "${ACT_SECRETS_FILE}" 2> /dev/null

oidc_app_id="$( get-value ".oidc.appId" )"
oidc_app_name="$( get-value ".oidc.name" )"
oidc_app_name="secret-${oidc_app_name}"

echo "Deleting the secret based credentials for the OIDC app '${oidc_app_name}'..."
key_id="$( az ad app credential list \
    --id "${oidc_app_id}" \
    --query "[?displayName=='${oidc_app_name}'].keyId | [0]" \
    --output tsv )"

if [[ -n $key_id ]]; then
    az ad app credential delete \
    --id "${oidc_app_id}" \
    --key-id "${key_id}"

    echo "Secret based credentials deleted."
else
    echo "No secret based credentials found."
fi
#!/usr/bin/env bash

# repo base
repo_base="$( git rev-parse --show-toplevel )"
base_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"

# shellcheck disable=SC1091
{
    source "${base_dir}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
    source "$SHARED_MODULE_DIR/act-credentials-module.sh"
}

# initialize az cli
initialize-az-cli "$HOME/.azure"

# remove locally cached secret
sudo rm "${ACT_SECRETS_FILE}" 2> /dev/null

# delete secret based credential in Azure AD app registration.
delete-secret-based-credentials
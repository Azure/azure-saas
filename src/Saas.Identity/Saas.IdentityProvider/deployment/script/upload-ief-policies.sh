#!/usr/bin/env bash

set -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/policy-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
}

"${SCRIPT_DIR}/generate-ief-policies.py" \
    "${CONFIG_FILE}" \
    "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE}" \
    "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_DIR}" |
    log-output \
        --level info \
        --header "Generating Identity Experience Framework Policies..." ||
    echo "Failed to generate IEF policies" |
    log-output \
        --level error \
        --header "Critical Error" ||
    exit 1

environment="$(get-value ".environment")"

# # set the user context to the service principal to run shell script to configure the Azure B2C policies
# service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
# set-user-context "${service_principal_username}"

dependency_sorted_array="$("${SCRIPT_DIR}/get-dependency-sorted-policies.py" \
    "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR}/${environment}")"

# iterate over each policy key in policy key array
readarray -t policy_file_array < <(jq --compact-output '.[]' <<<"${dependency_sorted_array}")

for policy in "${policy_file_array[@]}"; do
    id="$(jq --raw-output '.id' <<<"${policy}")"
    path="$(jq --raw-output '.path' <<<"${policy}")"

    # removing the BOM from the file or az rest will choke on it.
    # https://en.wikipedia.org/wiki/Byte_order_mark
    sed -i '1s/^\xEF\xBB\xBF//' "${path}"

    echo "Uploading policy '${id}' from '${path}'" |
        log-output \
            --level info

    upload-custom-policy "${id}" "${path}" ||
        echo "Failed to upload policy ${id}" |
        log-output \
            --level error \
            --header "Critical Error" ||
        exit 1

    echo "Policy ${id} successfully uploaded" |
        log-output \
            --level success
done

# resetting user context to the default User
reset-user-context

#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/policy-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/service-principal-module.sh"
}

b2c_tenant_id="$(get-value ".deployment.azureb2c.tenantId")"
service_principal_username="$(get-value ".deployment.azureb2c.servicePrincipal.username")"

credentials_path="$(get-user-value "${service_principal_username}" "credentialsPath")"
app_id="$(get-value ".deployment.azureb2c.servicePrincipal.appId")"

echo "Logging in with service principal..." |
    log-output \
        --level info \
        --header "Service Principal Login"

echo "Running the B2C Policy Script script in the context of the user ${service_principal_username} to allow a seperate az cli login session with the B2C tenant." |
    log-output \
        --level info

service-principal-login "${app_id}" "${credentials_path}" "${b2c_tenant_id}" ||
    (
        echo "Failed to login with service principal. Waiting 10 seconds and trying once more." |
            log-output \
                --level warning \
            ;
        sleep 10
        service-principal-login "${app_id}" "${credentials_path}" "${b2c_tenant_id}"
    ) ||
    echo "Failed to login with service principal for a second time. Try deleting previous service principals, then try to run this script again." |
    log-output \
        --level error \
        --header "Critical Error" ||
    exit 1

echo "Service principal login successful." | log-output --level success

policy_keys="$(get-value ".azureb2c.policyKeys")"

# iterate over each policy key in policy key array
readarray -t policy_key_array < <(jq --compact-output '.[]' <<<"${policy_keys}")

echo "Provisioning Policy Keys" | log-output --level info --header "Policy Keys"

for policy_key in "${policy_key_array[@]}"; do
    name="$(jq --raw-output '.name' <<<"${policy_key}")"
    options="$(jq --raw-output '.options' <<<"${policy_key}")"
    key_type="$(jq --raw-output '.keyType' <<<"${policy_key}")"
    key_use="$(jq --raw-output '.keyUsage' <<<"${policy_key}")"
    has_secret="$(jq --raw-output '.hasSecret' <<<"${policy_key}")"
    secret_path="$(jq --raw-output '.secretPath' <<<"${policy_key}")"

    if ! policy-key-exist "${name}"; then

        echo "Policy key ${name} does not exist. Creating it now..." |
            log-output --level info

        if [[ "${options}" == "Generate" ]]; then
            echo "Generating policy key" | log-output --level info

            create-policy-key-set "${name}" "${key_type}" "${key_use}" "${options}" "" ||
                echo "Failed to create policy key set" |
                log-output \
                    --level error \
                    --header "Critical Error" ||
                exit 1

            # echo "Waiting 10 seconds for key-set to settle..." | echo-color --level info
            # sleep 10

            # id="$(jq --raw-output '.id' <<<"${policy_key_body}")"

            # generate-policy-key "${id}" "${policy_key_body}"

        elif [[ "${options}" == "Manual" ]]; then
            echo "Adding manual policy key" | log-output --level info

            echo "Manual policy key has secret: ${has_secret}" | log-output --level info

            if [[ "${has_secret}" == true || "${has_secret}" == "true" ]]; then

                secret="$(cat "${secret_path}")" ||
                    echo "Failed to read secret from path: '${secret_path}'" |
                    log-output \
                        --level error \
                        --header "Critical Error" ||
                    exit 1

                create-policy-key-set "${name}" "${key_type}" "${key_use}" "${options}" "${secret}" ||
                    echo "Failed to create policy key set" |
                    log-output \
                        --level error \
                        --header "Critical Error" ||
                    exit 1

                # policy_key_body="$(create-policy-key-body "${name}" "${key_type}" "${key_use}" "${options}" "${secret}")"

                # create-policy-key-set "${policy_key_body}"

                # id="$(jq --raw-output '.id' <<<"${policy_key_body}")"

                # echo "Waiting 10 seconds for key-set to settle..." | echo-color --level info
                # sleep 10

                # upload-policy-secret "${id}" "${policy_key_body}"
            fi
        fi

        echo "Waiting 15 seconds before creating the next key-set..." | echo-color --level info
        sleep 15
    else
        echo "Policy key ${name} already exist." | log-output --level info
    fi
done

#!/bin/bash

set -u -e -o pipefail

# include script modules into current shell
source "./script/config-module.sh"
source "./script/policy-key-module.sh"
source "./script/log-module.sh"
source "./script/service-principal-module.sh"

b2c_tenant_id="$( get-value ".deployment.azureb2c.tenantId" )"
credentials_path="$( get-value ".deployment.azureb2c.servicePrincipal.credentialsPath" )"
app_id="$( get-value ".deployment.azureb2c.servicePrincipal.appId" )"

echo "Logging in with service principal..." \
    | log-output \
        --level info \
        --header "Service Principal Login"

service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"

echo "Running the B2C Policy Script script in the context of the user ${service_principal_username} to allow a seperate az cli login session with the B2C tenant." \
    | log-output \
        --level info

service-principal-login "${app_id}" "${credentials_path}" "${b2c_tenant_id}" \
    || ( echo "Failed to login with service principal. Waiting 10 seconds and trying once more." \
        | log-output \
            --level warning \
        ; sleep 10s ; service-principal-login "${app_id}" "${credentials_path}" "${b2c_tenant_id}" ) \
            || echo "Failed to login with service principal for a second time. Try deleting previous service principals, then try to run this script again." \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                | exit 1 

echo "Service principal login successful." | log-output --level success

policy_keys="$( get-value ".azureb2c.policyKeys" )"

# iterate over each policy key in policy key array
readarray -t policy_key_array < <( jq -c '.[]' <<< "${policy_keys}" )

echo "Provisioning Policy Keys" | log-output --level info --header "Policy Keys"

for policy_key in "${policy_key_array[@]}"; do
    name="$( jq -r '.name'              <<< "${policy_key}" )"
    options="$( jq -r '.options'        <<< "${policy_key}" )"
    key_type="$( jq -r '.keyType'       <<< "${policy_key}" )"
    key_use="$( jq -r '.keyUsage'       <<< "${policy_key}" )"
    has_secret="$( jq -r '.hasSecret'   <<< "${policy_key}" )"
    secret_path="$( jq -r '.secretPath' <<< "${policy_key}" )"

    if ! policy-key-exist "${name}"; then

        echo "Policy key ${name} does not exist. Creating it now..." \
            | log-output --level info

        if [[ "${options}" == "Generate" ]]; then
            echo "Generating policy key" | log-output --level info

            policy_key_body="$( create-policy-key-body "${name}" "${key_type}" "${key_use}" "${options}" "" )"

            create-policy-key-set "${policy_key_body}"

            echo "Waiting 10 seconds for key-set to settle..." | echo-color --level info
            sleep 10s

            id="$( jq -r '.id' <<< "${policy_key_body}" )"

            generate-policy-key "${id}" "${policy_key_body}"
            
        elif [[ "${options}" == "Manual" ]]; then
            echo "Adding manual policy key" | log-output --level info

            echo "Manual policy key has secret: ${has_secret}" | log-output --level info

            if [[ "${has_secret}" == true || "${has_secret}" == "true" ]]; then
                
                secret="$( cat "${secret_path}" )" \
                    || echo "Failed to read secret from path: '${secret_path}'" \
                        | log-output \
                            --level error \
                            --header "Critical Error" \
                            | exit 1

                policy_key_body="$( create-policy-key-body "${name}" "${key_type}" "${key_use}" "${options}" "${secret}")"

                create-policy-key-set "${policy_key_body}"

                id="$( jq -r '.id' <<< "${policy_key_body}" )"

                echo "Waiting 10 seconds for key-set to settle..." | echo-color --level info
                sleep 10s

                upload-policy-secret "${id}" "${policy_key_body}"
            fi
        fi

        echo "Waiting 15 seconds before creating the next key-set..." | echo-color --level info
        sleep 15s
    else
        echo "Policy key ${name} already exist." | log-output --level info
    fi
done
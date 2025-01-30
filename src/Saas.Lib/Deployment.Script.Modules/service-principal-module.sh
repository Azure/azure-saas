#!/usr/bin/env bash

# shellcheck disable=SC1091
# loading modules into current shell
source "$SHARED_MODULE_DIR/config-module.sh"
source "$SHARED_MODULE_DIR/user-module.sh"
source "$SHARED_MODULE_DIR/log-module.sh"

function create-app-role-assignment-body() {
    local principal_id="$1"
    local resource_id="$2"
    local app_role_id="$3"

    required_resource_access_json="$( cat <<-END
{
    "principalId": "{$principal_id}",
    "resourceId": "{$resource_id}",
    "appRoleId": "{$app_role_id}",
}
END
) "
        echo "${required_resource_access_json}"
        return
}

function service-principal-exist-by-name() {
    local service_principal_name="$1"

    if [[ -z "${service_principal_name}" \
        || "${service_principal_name}" == null \
        || "${service_principal_name}" == "null" ]]; then

        false
        return
    fi

    service_principal="$( az ad sp list \
        --display-name "${service_principal_name}" \
        --output tsv )"

    if [[ -n "${service_principal}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function service-principal-exist-by-id() {
    local service_principal_id="$1"

    if [[ -z "${service_principal_id}" \
        || "${service_principal_id}" == null \
        || "${service_principal_id}" == "null" ]]; then

        false
        return
    fi

    service_principal_response="$( az ad sp show \
        --id "${service_principal_id}" \
        --query id \
        --output tsv )"

    if [[ -n "${service_principal_response}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function is-permission-assigned() {
    local principal_object_id="$1"
    local permission_id="$2"

    result="$( az rest \
        --method GET \
        --url "https://graph.microsoft.com/v1.0/servicePrincipals/${principal_object_id}/appRoleAssignments" \
        --query "value \
            | [?appRoleId == '${permission_id}'].appRoleId
            | [0]" \
        --output tsv )"
    
    if [[ -n "${result}" && "${result}" == "${permission_id}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function assign-permissions-to-service-principal() {
    local app_id="$1"
    local app_role_name="$2"
    local ressource_app_id="$3"
    
    # add permission to service principal to read and write policy keys
    principal_object_id="$( az ad sp show \
        --id "${app_id}" \
        --query "id" \
        --output tsv  )"

    resource_id="$( az ad sp show \
        --id "${ressource_app_id}" \
        --query "id" \
        --output tsv )"

    app_role_id="$( az ad sp show \
        --id "${ressource_app_id}" \
        --query "appRoles[?value=='$app_role_name'].id" \
        --output tsv )"

    if is-permission-assigned "${principal_object_id}" "${app_role_id}"; then
        echo "Permission '${app_role_id}' already assigned to service principal." \
            | log-output \
                --level success
        return
    fi

    echo "Assigning '$app_role_name' permissions to service principal..." \
        | log-output --level info

    # create app role assignment body
    app_role_assignment_body="$( create-app-role-assignment-body \
        "${principal_object_id}" \
        "${resource_id}" \
        "${app_role_id}" )"

    # assign permission to service principal to read and write policy keys
    az rest --method POST \
            --uri "https://graph.microsoft.com/v1.0/servicePrincipals/${principal_object_id}/appRoleAssignments" \
            --headers "Content-Type=application/json" \
            --body "${app_role_assignment_body}" > /dev/null\
            || echo "Failed to assign permission to service principal to read and write policy keys" \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                || exit 1
}

function reset-sp-credentials() {
    local app_id="$1"
    local user_name="$2"

    key_set_rw_permission="TrustFrameworkKeySet.ReadWrite.All"
    policy_rw_permission="Policy.ReadWrite.TrustFramework"

    echo "Setting service principal permissions ${key_set_rw_permission}, ${policy_rw_permission}" \
            | log-output \
                --level info

    assign-permissions-to-service-principal \
        "${app_id}" \
        "${key_set_rw_permission}" \
        "${GRAPH_APP_ID}"\
        || echo "Could not set service principal permission ${key_set_rw_permission}." \
                | log-output \
                    --level "error" \
                    --header "Critical error" \
                    || exit 1

    echo "Permission ${key_set_rw_permission} set." \
            | log-output \
                --level success

    assign-permissions-to-service-principal \
        "${app_id}" \
        "${policy_rw_permission}" \
        "${GRAPH_APP_ID}"\
        || echo "Could not set service principal permission ${policy_rw_permission}." \
                | log-output \
                    --level "error" \
                    --header "Critical error" \
                    || exit 1

    echo "Permission ${policy_rw_permission} set." \
            | log-output \
                --level success
    
    echo "Resetting service principal credentials..." \
            | log-output \
                --level info

    # Set expiration time for certificate for tomorrow at the same time.
    # Note: We only need the service principal for a short time and although we're deleting both 
    # the certificate and the service principal as soon as we're done with them,
    # this is an extra precaution because of the power of the TrustFrameworkKeySet.ReadWrite.All permission.
    #expires="$( date -d "1 hour" +%Y-%m-%dT%H:%M:%S%:z )"
    expires="$( get-same-time-tomorrow )"

    # replace the password with a certificate that is set to expire soon.
    reset_response="$( az ad sp credential reset \
        --id "${app_id}" \
        --create-cert \
        --only-show-errors \
        --end-date "${expires}" )"

    # get certificate path of the certificate that was created
    certificate_path="$( jq --raw-output '.fileWithCertAndPrivateKey' <<< "${reset_response}" )"

    service_principal_context_dir="$( get-user-value "${user_name}" "contextDir" )"
    service_principal_credential_file="${service_principal_context_dir}/data/sp_credentials.pem"

    mkdir -p "${service_principal_context_dir}/data"

    # move certificate to service principal context dir
    mv "${certificate_path}" "${service_principal_credential_file}"

    if [[ -z "${certificate_path}" ]]; then
        echo "Could not find certificate path for service principal credentials certificate in response" \
            | log-output \
                --level error \
                --header "Critical error"
        exit 1
    fi

    # store certificate path in config
    put-user-value "${user_name}" "credentialsPath" "${service_principal_credential_file}"
}

function create-service-principal-for-policy-key-creation() {
    local service_principal_username="$1"

    if ! service-principal-exist-by-name "${service_principal_username}"; then
        echo "Creating service principal for ${service_principal_username}." | log-output --level info

        service_principal="$( az ad sp create-for-rbac \
            --name "${service_principal_username}" \
            --only-show-errors \
            || echo "Unable to create service principal." \
                        | log-output \
                            --level warning \
                            --header "Critical error" )"

        echo "Waiting 60 seconds for service principal to propagate ..." | log-output --level info
        sleep 60

        app_id="$( jq --raw-output '.appId' <<< "${service_principal}" )"
        put-value ".deployment.azureb2c.servicePrincipal.appId" "${app_id}"

        # setting service principal credentials
        reset-sp-credentials "${app_id}" "${service_principal_username}" \
            || echo "Could not set service principal credentials." \
                | log-output \
                    --level "error" \
                    --header "Critical error" \
                    || exit 1
    else

        echo "Service principal for policy key configuration already exist. Fetching details..." \
            | log-output \
                --level info

        service_principal_id="$( az ad sp list \
            --display-name "${service_principal_username}" \
            --query "[0].id" \
            --output tsv \
            || echo "Could not get principal principal details." \
                    | log-output \
                        --level "error" \
                        --header "Critical error" \
                        || exit 1 )"

        echo "Service principal id: ${service_principal_id}" \
            | log-output --level info

        service_principal="$( az ad sp show \
            --id "${service_principal_id}" )"

        app_id="$( jq --raw-output '.appId' <<< "${service_principal}" )"
        
        # resetting service principal credentials as the credentials are only valid for a short while
        reset-sp-credentials "${app_id}" "${service_principal_username}" \
            || echo "Could not set service principal credentials." \
                    | log-output \
                        --level "error" \
                        --header "Critical error" \
                        || exit 1
    fi
    
    return
}

function service-principal-login() {
    local app_id="$1"
    local credentials_path="$2"
    local b2c_tenant_id="$3"

    az login \
        --service-principal \
        --username "${app_id}" \
        --certificate "${credentials_path}" \
        --tenant "${b2c_tenant_id}" \
        --allow-no-subscriptions > /dev/null \
    || echo "Failed to login with service principal." \
        | log-output \
            --level warning \
            || exit 1
}

function delete-service-principal-credentials() {
    local app_id="$1"

    if [[ -z "${app_id}" || "${app_id}" == "null" ]]; then
        echo "No known service principal to delete." \
            | log-output \
                --level info
        return
    fi

    echo "Deleting service principal credentials for ${app_id}." \
        | log-output \
            --level info

    key_ids="$( az ad sp show \
        --id "${app_id}"\
        --query "keyCredentials" 2> /dev/null \
        || echo "Unable to delete service principal for ${app_id}, please delete it manually." \
            | log-output \
                --level warning )"

    # iterate over each key_id into an array
    readarray -t key_ids_array< <( jq --compact-output '.[]' <<< "${key_ids}" )

    if [[ "${#key_ids_array[@]}" -eq 0 ]]; then
        echo "No service principal credentials to delete." \
            | log-output \
                --level info
        return
    else
        principal_object_id="$( az ad sp show \
            --id "${app_id}" \
            --query "id" \
            --output tsv  )" \
            || echo "Failed to get service principal object id for appId ${app_id}" \
                | log-output \
                    --level warning \
                    --header "Warning!"
    fi

    # delete all service principal credentials
    for key_ids_array in "${key_ids_array[@]}"; do
        key_id="$( jq --raw-output '.keyId' <<< "${key_ids_array}" )"

        echo "Deleting service principal credentials with keyId: ${key_id} for ${principal_object_id}..." \
            | log-output --level info

        az ad sp credential delete \
            --id "${principal_object_id}" \
            --key-id "${key_id}" \
            --cert \
            | log-output \
            || echo "Failed to delete service principal credentials with keyId ${key_id} for appId ${app_id}. $?" \
                | log-output \
                    --level warning \
                    --header "Warning!"
    done
}



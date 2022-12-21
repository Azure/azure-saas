#!/bin/bash

# loading modules into current shell
source "./script/config-module.sh"
source "./script/linux-user-module.sh"
source "./script/log-module.sh"

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

function service-principal-exist() {
    local service_principal_username="$1"

    service_principal="$( az ad sp list \
        --display-name "${service_principal_username}" \
        --output tsv)"

    if [[ -n "${service_principal}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function assign-permissions-to-service-principal() {
    local app_id="$1"
    
    # add permission to service principal to read and write policy keys
    principal_id="$( az ad sp show \
        --id "${app_id}" \
        --query "id" \
        --output tsv  )"

    resource_id="$( az ad sp show \
        --id "00000003-0000-0000-c000-000000000000" \
        --query "id" \
        --output tsv )"

    app_role_id="$( az ad sp show \
        --id "00000003-0000-0000-c000-000000000000" \
        --query "appRoles[?value=='TrustFrameworkKeySet.ReadWrite.All'].id" \
        --output tsv )"

    # create app role assignment body
    app_role_assignment_body="$( create-app-role-assignment-body \
        "${principal_id}" \
        "${resource_id}" \
        "${app_role_id}" )"

    # assign permission to service principal to read and write policy keys
    az rest --method POST \
            --uri "https://graph.microsoft.com/v1.0/servicePrincipals/${principal_id}/appRoleAssignments" \
            --headers "Content-Type=application/json" \
            --body "${app_role_assignment_body}" \
            || echo "Failed to assign permission to service principal to read and write policy keys" \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                | exit 1
}

function reset-service-principal-credentials() {
    local app_id="$1"

    # Set expiration time for certificate to one hour from now.
    # Note: We only need the service principal for a short time and although we're deleting both 
    # the certificate and the service principal as soon as we're done with them,
    # this is an extra precaution because of the power of the TrustFrameworkKeySet.ReadWrite.All permission.
    expires="$( date -d "1 hour" +%Y-%m-%dT%H:%M:%S%:z )"

    # replace the password with a certificate that is set to expire soon.
    reset_response="$( az ad sp credential reset \
        --id "${app_id}" \
        --create-cert \
        --only-show-errors \
        --end-date "${expires}" )"

    # get certificate path of the certificate that was created
    certificate_path="$( jq -r '.fileWithCertAndPrivateKey' <<< "${reset_response}" )"

    if [[ -z "${certificate_path}" ]]; then
        echo "Could not find certificate path in response" \
            | log-output \
                --level error \
                --header "Critical error"
        exit 1
    fi

    # store certificate path in config
    put-value ".deployment.azureb2c.servicePrincipal.credentialsPath" "${certificate_path}"

    sleep 10s
}

function create-service-principal-for-policy-key-creation() {
    local service_principal_username="$1"

    if ! service-principal-exist "${service_principal_username}"; then
        echo "Creating service principal for policy key configuration ..." | log-output --level info

        service_principal="$( az ad sp create-for-rbac \
            --name "${service_principal_username}" \
            --only-show-errors \
            || echo "Unable to create service principal." \
                        | log-output \
                            --level warning \
                            --header "Critical error" )"

        echo "Waiting 30 seconds for service principal to propagate ..." | log-output --level info
        sleep 30s

        app_id="$( jq -r '.appId' <<< "${service_principal}" )"
        put-value ".deployment.azureb2c.servicePrincipal.appId" "${app_id}"

        # setting service principal credentials
        reset-service-principal-credentials "${app_id}" \
            || echo "Could not set service principal credentials." \
                | log-output \
                    --level "error" \
                    --header "Critical error" \
                    | exit 1


        assign-permissions-to-service-principal "${app_id}" \
            || echo "Could not set service principal credentials." \
                    | log-output \
                        --level "error" \
                        --header "Critical error" \
                        | exit 1
    else

        echo -e "Service principal for policy key configuration already exist. Fetching details..." | log-output

        service_principal_id="$( az ad sp list \
            --display-name "${service_principal_username}" \
            --query "[0].id" \
            --output tsv \
            || echo "Could not get principal principal details." \
                    | log-output \
                        --level "error" \
                        --header "Critical error" \
                        | exit 1)"

        echo -e "Service principal id: ${service_principal_id}" | log-output

        service_principal="$( az ad sp show \
            --id "${service_principal_id}" )"

        app_id="$( jq -r '.appId' <<< "${service_principal}" )"
        
        # resetting service principal credentials as the credentials are only valid for a short while
        reset-service-principal-credentials "${app_id}" \
            || echo "Could not set service principal credentials." \
                    | log-output \
                        --level "error" \
                        --header "Critical error" \
                        | exit 1
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
    --password "${credentials_path}" \
    --tenant "${b2c_tenant_id}" \
    --allow-no-subscriptions > /dev/null \
    || echo "Failed to login with service principal." \
        | log-output \
            --level warning \
            | exit 1
}

function delete-service-principal() {
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
    readarray -t key_ids_array< <( jq -c '.[]' <<< "${key_ids}" )

    # delete all service principal credentials
    for key_ids_array in "${key_ids_array[@]}"; do
        key_id="$( jq -r '.keyId' <<< "${key_ids_array}" )"

        echo "Deleting service principal credentials with keyId: ${key_id}..." \
            | log-output --level info

        az ad sp credential delete \
            --id "${app_id}" \
            --key-id "${key_id}" \
            --cert \
            | log-output \
            || echo "Failed to delete service principal credentials with keyId ${key_id} for appId ${app_id}. $?" \
                | log-output \
                    --level warning \
                    --header "Warning!"
    done
}



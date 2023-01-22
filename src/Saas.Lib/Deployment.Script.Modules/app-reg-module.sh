#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
source "$SHARED_MODULE_DIR/config-module.sh"
source "$SHARED_MODULE_DIR/log-module.sh"

function app-exist() {
    local app_id="$1"

    if [[ -z "${app_id}" \
        || "${app_id}" == null \
        || "${app_id}" == "null" ]]; then

        false
        return
    fi

    app_exist="$( az ad app show \
        --id "${app_id}" \
        --query "appId=='${app_id}'" 2> /dev/null \
        || false; return )"

    if [ "${app_exist}" == "true" ]; then
        true
        return
    else
        false
        return
    fi
}

function get-scope-permission-id()
{
    local resource_id="$1"
    local permission_name="$2"

    permission_id="$( az ad sp show \
        --id "${resource_id}" \
        --query "oauth2PermissionScopes[?value=='${permission_name}'].id" \
        --output tsv \
        || echo "Failed to get permission id for ${permission_name}" \
            || exit 1 )"

    echo "${permission_id}"
}

function get-app-role-permission-id()
{
    local resource_id="$1"
    local permission_name="$2"

    permission_id="$( az ad sp show \
        --id "${resource_id}" \
        --query "appRoles[?value=='${permission_name}'].id" \
        --output tsv \
        || echo "Failed to get permission id for ${permission_name}" \
            || exit 1 )"

    echo "${permission_id}"
}

function add-permission-scopes() {
    local obj_id="$1"
    local app_name="$2"
    local scopes="$3"

    # create persmission scopes json
    oauth_permissions_json=$( init-oauth-permissions )

    # read each item in the JSON array to an item in the Bash array
    readarray -t scope_array < <( jq --compact-output '.[]' <<< "${scopes}" )

    # iterate through the Bash array
    for scope in "${scope_array[@]}"; do
        scope_name=$( jq --raw-output '.name' <<< "${scope}" )
        scope_description=$( jq --raw-output '.description' <<< "${scope}" )

        # create permission json
        permission_json=$( create-oauth-permission \
            "${app_name}" \
            "${scope_name}" \
            "${scope_description}" )

        # add permission json to oauth permissions json
        oauth_permissions_json=$( jq --compact-output \
            --argjson permission_scope "${permission_json}" \
            '.api.oauth2PermissionScopes += [$permission_scope]' \
            <<< "${oauth_permissions_json}" )
    done

    # Microsoft Graph API for applications
    graph_url="https://graph.microsoft.com/v1.0/applications/${obj_id}"

    # add permissions to app registration using Microsoft Graph API
    az rest \
        --method "PATCH" \
        --uri "${graph_url}" \
        --body "${oauth_permissions_json}" \
        --only-show-errors \
        | log-output \
        || echo "Failed to add permissions: $?" \
                | log-output \
                    --level error \
                    --header "Critical error"
    return
}

function add-required-resource-access() {
    local permissions="$1"
    local app_id="$2"

    local endpoint
    local scope_name

    declare -i permission_scopes_length
    declare -i permission_app_roles_length

    required_resource_access_json_request=[]

    readarray -t permissions_array < <( jq --compact-output '.[]' <<< "${permissions}" )

    # iterate through the items in the array
    for permission in "${permissions_array[@]}"; do
        
        grant_admin_consent=$( jq --compact-output '.grantAdminConsent' <<< "${permission}" )

        # get the permission scopes
        permission_scopes=$(jq --raw-output '.scopes' <<< "${permission}")
        permission_scopes_length=$(jq --raw-output '.scopes | length' <<< "${permission}")

        permission_app_roles=$(jq --raw-output '.appRoles' <<< "${permission}")
        permission_app_roles_length=$(jq --raw-output '.appRoles | length' <<< "${permission}")
        
        # get the permission endpoint
        endpoint=$(jq --raw-output '.endpoint' <<< "${permission}")

        if [[ -n "${endpoint}" \
            && ! "${endpoint}" == "null" ]]; then
            resource_id=$( get-app-id "${endpoint}" )
            is_custom_resource=true
        else
            resource_id=$( jq --raw-output '.resourceId' <<< "${permission}" )
            is_custom_resource=false
        fi

        if [[ -n "${resource_id}" \
            || "${resource_id}" == null \
            || "${resource_id}" == "null" ]]; then
            
            echo "Resource id: '${resource_id}'" \
                | log-output \
                    --level info
        else
            echo "Custom resource." \
                | log-output \
                    --level info
        fi

        echo "Is custom resource: '${is_custom_resource}'" \
            | log-output \
                --level info

        # initialize required resource access json request
        required_resource_access_array_json="$( init-required-resource-access "${resource_id}" )"

        # adding permission scopes to required resource access json request
        if [[ -n $permission_scopes && ! $permission_scopes == null ]] \
            && [[ (( $permission_scopes_length -gt 0)) ]]; then

            required_resource_access_array_json="$( add-permission-scopes-to-required-access \
                "${permission_scopes}" \
                "${endpoint}" \
                "${resource_id}" \
                "${required_resource_access_array_json}" \
                "${is_custom_resource}" )"
        fi
    
        # adding permission app roles to required resource access json request
        if [[ -n $permission_app_roles && ! $permission_app_roles == null ]] \
            && [[ (( $permission_app_roles_length -gt 0)) ]]; then

            required_resource_access_array_json="$( add-permission-app-roles-to-required-access \
                "${permission_app_roles}" \
                "${endpoint}" \
                "${resource_id}" \
                "${required_resource_access_array_json}" \
                "${is_custom_resource}" )"
        fi

        required_resource_access_json_request="$( jq --raw-output \
            --argjson required_resource_access "${required_resource_access_array_json}" \
                '. += [$required_resource_access]' \
                    <<< "${required_resource_access_json_request}" )"
    done

    echo "Required Resource Accesses request: '${required_resource_access_json_request}'" \
        | log-output \
            --level info

    az ad app update \
        --id "${app_id}" \
        --required-resource-accesses "${required_resource_access_json_request}" \
        --only-show-errors \
        | log-output \
        || echo "Failed to add required resource access: $?" \
                | log-output \
                    --level error \
                    --header "Critical error"                

    if [[ "${grant_admin_consent}" == true  ]]; then

        echo "Waiting 60 seconds to allow the permissions to propagate before granting admin consent." \
            | log-output --level info

        sleep 60

        echo "Granting admin consent" | log-output --level info
        az ad app permission admin-consent \
            --id "${app_id}" \
            --only-show-errors \
            | log-output \
            || echo "Failed to grant admin consent: $?" \
                    | log-output \
                        --level error \
                        --header "Critical error"
    fi

    return
}

function add-permission-scopes-to-required-access() {
    local scopes="${1}"
    local endpoint="${2}"
    local resource_id="${3}"
    local required_resource_access_array_json="${4}"
    local is_custom_resource="${5}"

    echo "Permission scopes: '${scopes}'" \
        | log-output \
            --level info

    readarray -t scope_array < <( jq --compact-output '.[]' <<< "${scopes}" )

    if [[ ${#scope_array[@]} == 0 ]]; then
        echo "No scopes to add." \
            | log-output \
                --level info

        echo "${required_resource_access_array_json}"
        return
    fi

    # iterate through the scopes in the permission
    for scope_name in "${scope_array[@]}"; do

        # removing double quotes from scope name
        scope_name=$( jq --raw-output '.' <<< "${scope_name}" )

        if [[ $scope_name == null || -z $scope_name ]]; then
            continue
        fi

        if [[ "${is_custom_resource}" == "true" ]]; then
            scope_guid=$( get-scope-guid "${endpoint}" "${scope_name}" )
        else                   
            scope_guid=$( get-scope-permission-id  "${resource_id}" "${scope_name}" )
        fi

        echo "Adding scope name: '${scope_name}', Scope guid: '${scope_guid}'" \
            | log-output \
                --level info

        required_resource_access_json="$( create-required-resource-access "${scope_guid}" "Scope")"

        required_resource_access_array_json="$( jq --raw-output \
            --argjson required_resource_access "${required_resource_access_json}" \
                '.resourceAccess += [$required_resource_access]' \
                    <<< "${required_resource_access_array_json}" )"
    done

    echo "${required_resource_access_array_json}"
    return
}

function add-permission-app-roles-to-required-access() {
    local app_roles="${1}"
    local endpoint="${2}"
    local resource_id="${3}"
    local required_resource_access_array_json="${4}"
    local is_custom_resource="${5}"

    echo "Permission app roles: ${app_roles}" \
        | log-output \
            --level info

    readarray -t app_role_array < <( jq --compact-output '.[]' <<< "${app_roles}" )

    if [[ ${#app_role_array[@]} == 0 ]]; then
        echo "No app roles to add." \
            | log-output \
                --level info

        echo "${required_resource_access_array_json}"
        return
    fi
    
    # iterate through the scopes in the permission
    for app_role_name in "${app_role_array[@]}"; do

        # removing double quotes from app role name
        app_role_name=$( jq --raw-output '.' <<< "${app_role_name}" )

        if [[ $app_role_name == null || -z $app_role_name ]]; then
            continue
        fi

        if [[ "${is_custom_resource}" == "true" ]]; then
            app_role_guid=$( get-app-role-guid "${endpoint}" "${app_role_name}" )
        else
            app_role_guid=$( get-app-role-permission-id  "${resource_id}" "${app_role_name}" )
        fi

        echo "App role name: '${app_role_name}', App role guid: '${app_role_guid}'" \
            | log-output \
                --level info

        required_resource_access_json="$( create-required-resource-access "${app_role_guid}" "Role" )"

        required_resource_access_array_json="$( jq --raw-output \
            --argjson required_resource_access "${required_resource_access_json}" \
                '.resourceAccess += [$required_resource_access]' \
                    <<< "${required_resource_access_array_json}" )"

    done

    echo "${required_resource_access_array_json}"
    return
}

function create-required-resource-access() {
    local scope_guid="$1"
    local permission_type="$2"

    # create empty oauth permissions json
    required_resource_access_json="$( cat <<-END
{
    "id": "${scope_guid}",
    "type": "${permission_type}"
}
END
) " 
    echo "${required_resource_access_json}"
    return
}

function create-oauth-permission() {
    local app_name="$1"
    local scope_name="$2"
    local scope_description="$3"

    # get scope guid from deployment state if exists
    scope_guid=$( get-scope-guid "${app_name}" "${scope_name}" )

    # if scope guid does not exist, create a new guid and add it
    if [[ -z "${scope_guid}" || "${scope_guid}" == null ]]; then
        
        scope_guid=$(uuidgen)

        echo "Created new scope guid for scope: '${scope_name}' : '${scope_guid}'" \
            | log-output \
                --level info

        put-scope-guid "${app_name}" "${scope_name}" "${scope_guid}"
    fi

    # create permission json
    permission_json="$( cat <<-END
{
    "adminConsentDescription": "${scope_description}",
    "adminConsentDisplayName": "${scope_description}",
    "id": "${scope_guid}",
    "isEnabled": true,
    "type": "User",
    "userConsentDescription": "${scope_description}",
    "userConsentDisplayName": "${scope_description}",
    "value": "${scope_name}"
} 
END
)"

    echo "${permission_json}"
    return
}

function init-oauth-permissions() {

    oauth_permissions_json="$( cat <<-END
{   "api": {
        "oauth2PermissionScopes": [
        ] 
    }
}
END
)"
        echo "${oauth_permissions_json}"
        return
}

function init-required-resource-access() {
    local resource_id="$1"

    required_resource_access_json="$( cat <<-END
{
    "resourceAppId": "${resource_id}",
    "resourceAccess": [
    ]
}
END
) "
        echo "${required_resource_access_json}"
        return
}
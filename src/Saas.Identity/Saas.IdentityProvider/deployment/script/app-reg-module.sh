#!/bin/bash


# include script modules into current shell
source "./script/config-module.sh"
source "./script/log-module.sh"

function app-exist() {
    local app_id="$1"

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

function get-permission-id()
{
    local resource_id="$1"
    local permission_name="$2"

    permission_id="$( az ad sp show \
        --id "${resource_id}" \
        --query "oauth2PermissionScopes[?value=='${permission_name}'].id" \
        --output tsv \
        || echo "Failed to get permission id for ${permission_name}" | exit 1 )"

    echo "${permission_id}"
}

function add-permission-scopes() {
    local obj_id="$1"
    local app_name="$2"
    local scopes="$3"

    # create persmission scopes json
    oauth_permissions_json=$( init-oauth-permissions )

    # read each item in the JSON array to an item in the Bash array
    readarray -t scope_array < <( jq -c '.[]' <<< "${scopes}" )

    # iterate through the Bash array
    for scope in "${scope_array[@]}"; do
        scope_name=$( jq -r '.name' <<< "${scope}" )
        scope_description=$( jq -r '.description' <<< "${scope}" )

        # create permission json
        permission_json=$( create-oauth-permission \
            "${app_name}" \
            "${scope_name}" \
            "${scope_description}" )

        # echo "Permission json: ${permission_json}"

        # add permission json to oauth permissions json
        oauth_permissions_json=$( jq -c \
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

    # iterate through the items in the array
    for item in "${permissions[@]}"; do

        # get the permission
        permission=$( jq -c '.[]' <<< "${item}" )
        grant_admin_consent=$( jq -r '.grantAdminConsent' <<< "${permission}" )

        # get the permission scopes
        scopes=$(jq '.scopes' <<< "${permission}")
        
        # get the permission endpoint
        endpoint=$(jq -r '.endpoint' <<< "${permission}")


        if [[ -n "${endpoint}" \
            && ! "${endpoint}" == "null" ]]; then
            resource_id=$( get-app-id "${endpoint}" )
            is_custom_resource=true
        else
            resource_id=$( jq -r '.resourceId' <<< "${permission}" )
            is_custom_resource=false
        fi

        echo "Is custom resource: '${is_custom_resource}'" | log-output --level info
        echo "Resource id: '${resource_id}'" | log-output --level info

        required_resource_access_array_json="$( init-required-resource-access "${resource_id}" )"

        readarray -t scope_array < <( jq -r '.[]' <<< "${scopes}" )

        # iterate through the scopes in the permission
        for scope_name in "${scope_array[@]}"; do

            if [[ "${is_custom_resource}" == "true" ]]; then
                scope_guid=$( get-scope-guid "${endpoint}" "${scope_name}" )
            else
                scope_guid=$( get-permission-id  "${resource_id}" "${scope_name}" )
            fi

            echo "Scope name: '${scope_name}', Scope guid: '${scope_guid}'" | log-output --level info

            required_resource_access_json="$( create-required-resource-access "${scope_guid}" )"

            required_resource_access_array_json="$( jq -c \
                --argjson required_resource_access "${required_resource_access_json}" \
                '.[0].resourceAccess += [$required_resource_access]' \
                <<< "${required_resource_access_array_json}" )"

        done
        
        az ad app update \
            --id "${app_id}" \
            --set "requiredResourceAccess=${required_resource_access_array_json}" \
            --only-show-errors \
            | log-output \
            || echo "Failed to add required resource access: $?" \
                    | log-output \
                        --level error \
                        --header "Critical error"

        echo "Grant admin consent: ${grant_admin_consent}" | log-output --level info

        if [[ "${grant_admin_consent}" == true  ]]; then

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

    done

    return
}

function create-required-resource-access() {
    local scope_guid="$1"

    # create empty oauth permissions json
    required_resource_access_json="$( cat <<-END
{
    "id": "${scope_guid}",
    "type": "Scope"
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
    if [[ -z "${scope_guid}" || "${scope_guid}" == "null" ]]; then
        scope_guid=$(uuidgen)
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
) "
        echo "${oauth_permissions_json}"
        return
}

function init-required-resource-access() {
    local resource_id="$1"

    required_resource_access_json="$( cat <<-END
[{
    "resourceAppId": "${resource_id}",
    "resourceAccess": [
    ]
}]
END
) "
        echo "${required_resource_access_json}"
        return
}
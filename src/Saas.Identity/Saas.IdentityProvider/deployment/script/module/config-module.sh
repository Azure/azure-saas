#!/usr/bin/env bash

# loading modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/colors-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"

function set-version() {
    local ver="$1"
    put-value ".version" "${ver}"
}

# check if config file exists and create it if not
function config-file-exists-or-exit() {
    if [[ ! -f "${CONFIG_FILE}" ]]; then
        cp "${CONFIG_TEMPLATE_FILE}" "${CONFIG_FILE}"
        exit 1
    fi
}

function exist-or-exit() {
    local json_path="$1";
    local value;

    value="$( get-value "${json_path}" )"

    if [[ -z "${value}"  ]] ; then
        
        echo "Please configure ${json_path} in file ${CONFIG_FILE}." \
            | log-output \
                --level error \
                --header "Critical Error"

        echo "$value" "$json_path"

        exit 1
    fi

    echo "$value" "$json_path"
}

function get-value { 
    local key="$1" ;

    local json ;
    
    json="$( cat "${CONFIG_FILE}" )" ;
    echo "${json}" | jq -r "${key}"
}

function get-object { 
    local key="$1" ;

    local json ;
    
    json="$( cat "${CONFIG_FILE}" )" ;
    echo "${json}" | jq -r "${key}"
}

function put-value { 
    local key="$1" ;
    local variableValue="$2" ;
    local json ;

    json="$( cat "${CONFIG_FILE}" )" ;

    output="$( echo "${json}" \
       | jq --arg x "${variableValue}" \
            "${key}=(\$x)" \
       )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-public-key-path() {
    local app_name="$1"

    local json

    json="$( cat "${CONFIG_FILE}" )"
    
    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).publicKeyPath"
}

function put-key-certificate-path() {
    local app_name="$1"
    local key_path="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${key_path}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).publicKeyPath \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-key-certificate-path() {
    local app_name="$1"
    local key_path="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${key_path}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).publicKeyPath \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function put-certificate-key-name() {
    local app_name="$1"
    local key_name="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${key_name}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).certificateKeyName \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-certificate-key-name() {
    local app_name="$1"
    local key_name="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).certificateKeyName" 
}

function get-policy-key-secret-path() {
    local name="$1"
    local json

    json="$( cat "${CONFIG_FILE}" )"
    
    echo "${json}" \
        | jq -r \
            "( .azureb2c.policyKeys[] \
            | select(.name==\"${name}\") ).secretPath"
}

function put-policy-key-secret-path() {
    local name="$1"
    local secret_path="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${secret_path}" \
            "( .azureb2c.policyKeys[] \
            | select(.name==\"${name}\") ).secretPath \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-app-id() {
    local app_name="$1"
    local json
    json="$( cat "${CONFIG_FILE}" )"
    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).appId"
}

function put-app-id() {
    local app_name="$1"
    local app_id="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${app_id}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).appId \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function put-app-object-id() {
    local app_name="$1"
    local object_id="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${object_id}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).objectId \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

get-app-value() {
    local app_name="$1"
    local key="$2"
    local json

    json="$( cat "${CONFIG_FILE}" )"
    
    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).${key}"
}

put-app-value() {
    local app_name="$1"
    local key="$2"
    local value="$3"
    
    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${value}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).${key} \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-scope-guid() {
    local app_name="$1"
    local scope_name="$2"
    local json
    json="$( cat "${CONFIG_FILE}" )"
    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") \
            | .scopes[] \
            | select(.name==\"${scope_name}\") ).guid" 
}

function get-app-role-guid() {
    local app_name="$1"
    local app_role_name="$2"
    
    local json

    json="$( cat "${CONFIG_FILE}" )"
    echo "${json}" \
        | jq -r \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") \
            | .appRoles[] \
            | select(.name==\"${app_role_name}\") ).guid" 
}

function put-scope-guid() {
    local app_name="$1"
    local scope_name="$2"
    local scope_guid="$3"
    local json
    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq --arg x "${scope_guid}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") \
            | .scopes[] \
            | select(.name==\"${scope_name}\") ).guid \
            |= \$x" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-value-file { 
    local file_name="$1" 
    local key="$2" ;
    local json ;
    
    json="$( cat "${file_name}" )" ;

    echo "${json}" | jq -r "${key}"
}

function put-value-file { 
    local file_name="$1"   
    local key="$2" ;
    local value="$2" ;
    local json ;

    json="$( cat "${file_name}" )" ;

    output="$( echo "${json}" \
       | jq --arg x "${value}" \
            "${key}=(\$x)" \
       )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function put-certificate-value { 
    local key="$1" ;
    local value="$2" ;
    local json ;

    json="$( cat "${CERTIFICATE_POLICY_FILE}" )" ;

    regex_is_number='^[0-9]+$'
    regex_is_boolean='^(true|True|false|False)$'

    # Determin if a value is an integer or a string before writing to json
    if [[ $value =~ $regex_is_number ]]; then
        output="$( echo "${json}" \
            | jq --arg x "${value}" \
                "${key}=(\$x|tonumber)" \
                )" \
                && echo "${output}" > "${CERTIFICATE_POLICY_FILE}" \
                || exit 1
    elif [[ $value =~ $regex_is_boolean ]]; then
        output="$( echo "${json}" \
            | jq --arg x "${value}" \
                "${key}=(\$x | ascii_downcase | test(\"true\") )" \
            )" \
                && echo "${output}" > "${CERTIFICATE_POLICY_FILE}" \
                || exit 1
    else
        output="$( echo "${json}" \
            | jq --arg x "${value}" "${key}=(\$x)" \
            )" \
                && echo "${output}" > "${CERTIFICATE_POLICY_FILE}" \
                || exit 1
    fi    
}

function get-user() {
    local user_name="$1"

    json="$( cat "${CONFIG_FILE}" )"

    echo "${json}" \
        | jq -r \
            --arg name "${user_name}" \
            ".deployment.users[] \
            | select(.name == \$name)"
}

function get-user-value() {
    local user_name="$1"
    local key="$2"

    json="$( cat "${CONFIG_FILE}" )"

    echo "${json}" \
        | jq -r \
            --arg name "${user_name}" \
            --arg key "${key}" \
            ".deployment.users[] \
            | select(.name == \$name) \
            | .[\$key]"
}

function put-user-value() {
    local user_name="$1"
    local key="$2"
    local value="$3"

    json="$( cat "${CONFIG_FILE}" )"

    regex='^[0-9]+$'

    if [[ $value =~ $regex ]] ; then
        output="$( echo "${json}" \
        | jq -r \
            --arg val "${value}" \
            --arg name "${user_name}" \
                ".deployment.users \
                |= (map(.name) \
                | index(\$name)) as \$ix \
                | if \$ix \
                    then .[\$ix] += { ${key}: \$val|tonumber } \
                    else . \
                end" \
            )" \
                && echo "${output}" > "${CONFIG_FILE}" \
                || exit 1
    else
        output="$( echo "${json}" \
        | jq -r \
            --arg val "${value}" \
            --arg name "${user_name}" \
                ".deployment.users \
                |= (map(.name) \
                | index(\$name)) as \$ix \
                | if \$ix \
                    then .[\$ix] += { ${key}: \$val } \
                    else . \
                end" \
            )" \
                && echo "${output}" > "${CONFIG_FILE}" \
                || exit 1
    fi
}

function put-user-context() {
    local user_name="$1"
    local user_context_dir="$2"

    local json

    json="$( cat "${CONFIG_FILE}" )"

    output="$( echo "${json}" \
        | jq -r \
            --arg name "${user_name}" \
            --arg dir "${user_context_dir}" \
                ".deployment.users \
                |= (map(.name) \
                | index(\$name)) as \$ix \
                | if \$ix \
                    then .[\$ix] += {contextDir: \$dir} \
                    else . + [{name: \$name, contextDir: \$dir}]
                end" \
            )" \
                && echo "${output}" > "${CONFIG_FILE}" \
                || exit 1
}

function put-json-value { 
    local key="$1" ;
    local variableValue="$2" ;
    local json ;

    json="$( cat "${CONFIG_FILE}" )" ;
    
    output="$(echo "${json}" \
       | jq --arg x "${variableValue}" "${key}=(\$x | fromjson)" \
        )" \
            && echo "${output}" > "${CONFIG_FILE}" \
            || exit 1
}

function get-user-context-dir() {
    local user_name="$1"

    json="$( cat "${CONFIG_FILE}" )"

    echo "${json}" \
        | jq -r \
            --arg name "${user_name}" \
            ".deployment.users[] \
            | select(.name == \$name) \
            | .contextDir"
}
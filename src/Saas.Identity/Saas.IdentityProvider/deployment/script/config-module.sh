#!/bin/bash

# loading modules into current shell
source "./script/constants-module.sh"
source "./script/colors-module.sh"
source "./script/log-module.sh"

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

function put-value { 
    local key="$1" ;
    local variableValue="$2" ;
    local json ;
    json="$( cat "${CONFIG_FILE}" )" ;
    echo "${json}" \
       | jq --arg x "${variableValue}" \
            "${key}=(\$x)" \
       > "${CONFIG_FILE}"
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

function put-public-key-path() {
    local app_name="$1"
    local public_key_path="$2"
    local json
    json="$( cat "${CONFIG_FILE}" )"
    echo "${json}" \
        | jq --arg x "${public_key_path}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).publicKeyPath \
            |= \$x" \
        > "${CONFIG_FILE}"
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
    echo "${json}" \
        | jq --arg x "${secret_path}" \
            "( .azureb2c.policyKeys[] \
            | select(.name==\"${name}\") ).secretPath \
            |= \$x" \
        > "${CONFIG_FILE}"
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
    echo "${json}" \
        | jq --arg x "${app_id}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") ).appId \
            |= \$x" \
        > "${CONFIG_FILE}"
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

function put-scope-guid() {
    local app_name="$1"
    local scope_name="$2"
    local scope_guid="$3"
    local json
    json="$( cat "${CONFIG_FILE}" )"
    echo "${json}" \
        | jq --arg x "${scope_guid}" \
            "( .appRegistrations[] \
            | select(.name==\"${app_name}\") \
            | .scopes[] \
            | select(.name==\"${scope_name}\") ).guid \
            |= \$x" \
        > "${CONFIG_FILE}"
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
    local variableValue="$2" ;
    local json ;
    json="$( cat "${file_name}" )" ;
    echo "${json}" \
       | jq --arg x "${variableValue}" \
            "${key}=(\$x)" \
       > "${CONFIG_FILE}"
}

function put-certificate-value { 
    local key="$1" ;
    local variableValue="$2" ;
    local json ;

    json="$( cat "${CERTIFICATE_POLICY_FILE}" )" ;

    regex='^[0-9]+$'

    # Determin if a value is an integer or a string before writing to json
    if [[ $variableValue =~ $regex ]] ; then
        echo "${json}" \
            | jq --arg x "${variableValue}" "${key}=(\$x|tonumber)" \
            > "${CERTIFICATE_POLICY_FILE}"
    else
        echo "${json}" \
            | jq --arg x "${variableValue}" "${key}=(\$x)" \
            > "${CERTIFICATE_POLICY_FILE}"
    fi    
}

function put-json-value { 
    local key="$1" ;
    local variableValue="$2" ;
    local json ;
    json="$( cat "${CONFIG_FILE}" )" ;
    echo "${json}" \
       | jq --arg x "${variableValue}" "${key}=(\$x | fromjson)" \
       > "${CONFIG_FILE}"
}
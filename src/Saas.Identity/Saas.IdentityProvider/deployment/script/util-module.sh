#!/bin/bash

source "./script/log-module.sh"

function value-exist() {
    local json_path="$1";

    value="$( get-value "${json_path}" )"

    if [[ -z "${value}" ]] ; then
        
        echo "Value of ${json_path} is missing." \
            | log-output \
                --level error \
                --header "Critical Error"

        exit 1
    fi

    echo "$value"
}

function is-guid() {
    local json_path="$1";
    local value;

    value="$( value-exist "${json_path}" )" || exit 1

    if ! [[ "${value}" =~ ^\{?[A-F0-9a-f]{8}-[A-F0-9a-f]{4}-[A-F0-9a-f]{4}-[A-F0-9a-f]{4}-[A-F0-9a-f]{12}\}?$ ]]; then
        echo "The value '${value}' of '${json_path}' is not a valid GUID." \
            | log-output \
                --level error
        exit 1
    else

        #echo "$value"
        true
        return
    fi 
}

function is-valid-location() {
    local json_path="$1";
    local value;

    value="$( value-exist "${json_path}" )" || exit 1

    is_valid_location="$( az account list-locations \
        --query "[?name=='${value}'].name" \
        --output tsv )"

    if [[ -z "${is_valid_location}" ]]; then
        echo "The value '${value}' of '${json_path}' is not a valid location." \
            | log-output \
                --level error
        exit 1
    else
        #echo "$value"
        true
        return
    fi    
}

function is-valid-b2c-location()
{
        local json_path="$1";
    local value;

    value="$( value-exist "${json_path}" )" || exit 1

    if ! [[ "${value}" == "United States" \
            || "${value}" == "Europe" \
            || "${value}" == "Australia" \
            || "${value}" == "Asia Pacific" ]]; then
        echo "The value '${value}' of '${json_path}' is not a valid B2C location." \
            | log-output \
                --level error
        exit 1
    else
        true
        return
    fi 
}

function is-valid-b2c-tier() {
    local json_path="$1";
    local value;

    value="$( value-exist "${json_path}" )" || exit 1

    if ! [[ "${value}" == "A0" ]]; then
        echo "The value '${value}' of '${json_path}' is not a valid B2C tier." \
            | log-output \
                --level error
        exit 1
    else
        true
        return
    fi    
}

function is-valid-b2c-sku() {
    local json_path="$1";
    local value;

    value="$( value-exist "${json_path}" )" || exit 1

    if ! [[ "${value}" == "Standard" \
            || "${value}" == "PremiumP1" \
            || "${value}" == "PremiumP2" ]]; then
        echo "The value '${value}' of '${json_path}' is not a valid B2C Sku." \
            | log-output \
                --level error
        exit 1
    else
        true
        return
    fi 
}
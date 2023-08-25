#!/usr/bin/env bash

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

    value="$( value-exist "${json_path}" )" \
        || exit 1

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

    value="$( value-exist "${json_path}" )" \
        || exit 1

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

function get-os() {
    if [[ "${OSTYPE}" == "linux-gnu"* ]]; then
        echo "linux"
        return
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        echo "macos"
        return
    else
        exit 1
    fi
}

function is-valid-bash() {
    local minumum_bash_version=$1
    local bash_version;

    bash_version="$( bash --version | head -n 1 | cut -d " " -f 4 )" \
        || echo "Fail: Bash is not installed" || exit 1

    test-version-compare "${bash_version}" "${minumum_bash_version}" ">"
}

function is-valid-az-cli() {
    local minumum_version=$1
    local az_cli_version;

    if ! command -v az &> /dev/null; then
        echo "Fail: jq is not installed."
        exit 1
    fi

    az_cli_version="$( az version --query "\"azure-cli\"" --output tsv )" \
        || echo "Fail: Cannot determine Azure CLI version." || exit 1

    test-version-compare "${az_cli_version}" "${minumum_version}" ">"
}

function is-valid-jq() {
    local minumum_version=$1
    local jq_version;

    if ! command -v jq &> /dev/null; then
        echo "Fail: jq is not installed."
        exit 1
    fi

    jq_version="$( jq --version | head -n 1 | cut -c 4- )" \
        || echo "Fail: Cannot determine jq version." || exit 1

    test-version-compare "${jq_version}" "${minumum_version}" ">"
}

function is-mkpasswd-valid() {
    local minumum_version=$1

    if ! command -v mkpasswd &> /dev/null; then
        echo "Fail: mkpasswd is not installed."
        exit 1
    fi

    mkpasswd_version="$( mkpasswd --version | head -n 1 | cut -c 10-16 )" \
        || echo "Fail: Cannot determine mkpasswd version." || exit 1

    test-version-compare "${mkpasswd_version}" "${minumum_version}" ">"
}

test-version-compare() {
    local ver1=$1
    local ver2=$2

    version-compare "${ver1}" "${ver2}"

    case $? in
        0) op='=';;
        1) op='>';;
        2) op='<';;
    esac

    # shellcheck disable=SC2053
    if [[ $op != $3 ]]; then
        echo "Fail: version must be higher than $ver2"
        exit 1
    else
        echo "Pass: '$1 $op $2'"
    fi
}

version-compare() {
    local ver1=$1
    local ver2=$2


    if [[ "${ver1}" == "${ver2}" ]]
    then
        return 0
    fi
    local IFS=.

    # shellcheck disable=SC2206
    local i ver1=($ver1) 
    # shellcheck disable=SC2206
    local i ver2=($ver2)
    
    # fill empty fields in ver1 with zeros
    for ((i=${#ver1[@]}; i<${#ver2[@]}; i++))
    do
        ver1[i]=0
    done

    for ((i=0; i<${#ver1[@]}; i++))
    do
        if [[ -z ${ver2[i]} ]]; then
            # fill empty fields in ver2 with zeros
            ver2[i]=0
        fi
        if ((10#${ver1[i]} > 10#${ver2[i]})); then
            return 1
        fi
        if ((10#${ver1[i]} < 10#${ver2[i]})); then
            return 2
        fi
    done

    return 0
}

function user-id-exist() {
    local usr_id="$1"

    is_number_reg_ex='^[0-9]+$'

    if ! [[ $usr_id =~ $is_number_reg_ex ]] ; then
        echo "User id must be a number" >&2
        exit 1
    fi

    id "${usr_id}" &> /dev/null \
        && return 0 \
        || return 1
}

function create-random-password() {
    local passwd

    # shellcheck disable=SC2002
    passwd="$( cat /dev/urandom \
        | LC_ALL=C tr -dc 'a-zA-Z0-9!#&*;@^_{}~' \
        | fold -w 24 \
        | head -n 1 )"

    echo "${passwd}"
    return
}

function user-exist() {
    local usr_name="$1"

    if id "${usr_name}" > /dev/null 2>&1 ; then
        true
        return
    else
        false
        return
    fi
}

function user-group-exist() {
    local usr_grp="$1"

    if groups "${usr_grp}" > /dev/null 2>&1 ; then
        true
        return
    else
        false
        return
    fi
}

function is-valid-context-dir() {
    local context_dir="$1"
    local user_name="$2"

    if [[ -d "${context_dir}" \
        && "${context_dir}" == *"${user_name}"*
        && ! "${context_dir}" == "${HOME}/" \
        && ! "${context_dir}" == "${HOME}"
        && ! "${context_dir}" == "/" ]]; then
        true
        return
    else
        false
        return
    fi
}

function preserve-azure-cli-context() {

    azure_cli_config_dir="$( get-value ".deployment.azureCli.configDir" )"

    if [[ "${azure_cli_config_dir}" == null ]]; then
        echo "Preserving Azure CLI context" \
            | log-output \
                --level info \
                --header "Azure CLI" 

        # Check if the environment variable AZURE_CONFIG_DIR is set and if not set it to the default value.
        # set +u to prevent the script from exiting if the variable is not set.
        set +u
        if [[ -z "${AZURE_CONFIG_DIR}" ]]; then
            put-value ".deployment.azureCli.configDir" "${HOME}/.azure"
            export AZURE_CONFIG_DIR="${HOME}/.azure"
        else
            put-value ".deployment.azureCli.configDir" "${AZURE_CONFIG_DIR}"
        fi
        # Re-enable the exit on unset variable.
        set -u
    fi
}

function restore-azure-cli-context() {
    azure_cli_config_dir="$( get-value ".deployment.azureCli.configDir" )"

    if ! [[ "${azure_cli_config_dir}" == null ]]; then
        export AZURE_CONFIG_DIR="${azure_cli_config_dir}"
    else
        export AZURE_CONFIG_DIR="${HOME}/.azure"
    fi
}

function set-azure-cli-context() {
    local context_dir="$1"

    if [[ -n "${context_dir}" ]]; then
        export AZURE_CONFIG_DIR="${context_dir}"
    fi
}
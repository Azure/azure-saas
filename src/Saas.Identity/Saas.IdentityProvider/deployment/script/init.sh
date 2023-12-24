#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/util-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/backup-module.sh"
}

function check-prerequisites() {

    echo "Checking prerequisites..." |
        log-output \
            --level info \
            --header "Checking prerequisites"

    what_os="$(get-os)" ||
        echo "Unsupported OS: ${what_os}. This script support linux (WSL 2.0 on Windows) and MacOS." |
        log-output \
            --level error \
            --header "Critical Error" ||
        exit 1

    echo "Supported operating system: ${what_os}" |
        log-output \
            --level success \
            --header "Checking OS"

    if [[ "${GIT_ORG_PROJECT_NAME}" == "Azure/azure-saas" ]]; then
        echo "To run this script you need to fork it to your own repository first." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    else
        echo "Forked repository true: ${GIT_ORG_PROJECT_NAME}" |
            log-output \
                --level success \
                --header "Checking Repo forked"
    fi

    # check if bash version is supported
    is-valid-bash "5.0.0" |
        log-output \
            --level success \
            --header "Checking bash version" ||
        echo "The version of bash is not supported. Try updating the latest version." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # check if az cli version is supported
    is-valid-az-cli "2.46.0" |
        log-output \
            --level success \
            --header "Checking az cli version" ||
        echo "The version of az cli is not supported. Try updating to the latest version." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    is-valid-jq "1.5" |
        log-output \
            --level success \
            --header "Checking jq version" ||
        echo "The version of jq is not supported. Try updating to the latest version." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # if running in a container copy the msal token cache
    # so that user may not have to log in again to main tenant.
    if [ -f /.dockerenv ]; then
        cp -f /asdk/.azure/msal_token_cache.* /root/.azure/
        cp -f /asdk/.azure/azureProfile.json /root/.azure/
    fi
}

function initialize-configuration-manifest-file() {

    if [[ ! -s "${CONFIG_FILE}" || ! -f "${CONFIG_FILE}" ]]; then
        echo "It looks like this is the first time you're running this script. Setting things up..." |
            log-output \
                --level info

        echo

        echo "Creating new './config/config.json' from 'config-template.json.'" |
            log-output \
                --level info

        cp "${CONFIG_TEMPLATE_FILE}" "${CONFIG_FILE}"
        sudo chown -R 666 "${CONFIG_DIR}"

        echo

        echo "Before beginning deployment you must specify initial configuration in the 'initConfig' object:" |
            log-output \
                --level warning

        init_config="$(get-value ".initConfig")"

        echo "${init_config}" |
            log-output \
                --level msg

        echo

        echo "Please add required initial settings to the initConfig object in ./config/config.json and run this script again." |
            log-output \
                --level warning
        exit 2

    else

        # Setting configuration variables
        echo "Initializing Configuration" |
            log-output \
                --level info \
                --header "Configation Settings"

        backup-config-beginning
    fi

    echo "Configuration settings: $CONFIG_FILE." |
        log-output \
            --level success
}

# check if prerequisites for running the deployment script are met
check-prerequisites

# initialize configuration manifest file
initialize-configuration-manifest-file

# set to install az cli extensions without prompting
az config set extension.use_dynamic_install=yes_without_prompt &>/dev/null

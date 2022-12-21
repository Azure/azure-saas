#!/bin/bash

# include script modules into current shell
source "./script/colors-module.sh"
source "./script/tenant-login-module.sh"
source "./script/config-module.sh"
source "./script/linux-user-module.sh"
source "./script/log-module.sh"
source "./script/util-module.sh"

function final-state() {
    resource_group_state="$( get-value ".deployment.resourceGroup.provisionState" )"
    key_vault_state="$( get-value ".deployment.keyVault.provisionState" )"
    azure_b2c_state="$( get-value ".deployment.azureb2c.provisionState" )"
    azure_b2c_config_state="$( get-value ".deployment.azureB2C.configurationState" )"

    echo 
    if [ "${resource_group_state}" == "successful" ] \
        && [ "${key_vault_state}" == "successful" ] \
        && [ "${azure_b2c_state}" == "successful" ] \
        && [ "${azure_b2c_config_state}" == "successful" ]; then
        echo "Deployment script completed successfully." \
            | log-output \
                --level success \
                --header "Deployment script completion"
            exit 0
    else
        echo "Deployment script completed with errors." \
            | log-output \
                --level error \
                --header "Deployment script completion"
            exit 1
    fi
}

function clean-up() {
    set  +e
    check-settings || exit 1
    set -e

    echo "Starting cleaning up..." \
        | log-output \
            --level info \
            --header "Cleaning up"

    echo 
    b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"

    echo "Deleting locallay stored service principal credentials." \
        | log-output \
        --level info

    sudo runuser -u "${b2c_config_usr_name}" "./shell-delete-service-principal.sh"

    service_principal_home_dir="$( get-value ".deployment.azureb2c.servicePrincipal.userHomeDir" )"
    service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"

    # deleting the temp service principal user as we no longer need it.
    clean-up-linux-user "${service_principal_username}" "${service_principal_home_dir}"
    echo "User ${service_principal_username} have been deleted." | log-output --level success

    b2c_config_usr_home_dir="$( get-value ".deployment.azureb2c.userHomeDir" )"

    # Deleting the temp b2c configuration user as we no longer need it.
    clean-up-linux-user "${b2c_config_usr_name}" "${b2c_config_usr_home_dir}"
    echo "User ${b2c_config_usr_name} have been deleted." | log-output --level success
    echo
    echo "Clean up has completed." | log-output --level success

    final-state
}

function check-settings() {
    # check to see if all initial configuration values exist in config.json, exit if not.
    echo "Validating Initial Configuration Settings..." \
        | log-output \
            --level info \
            --header "Configuration Validation"

    ( 
         is-guid ".initConfig.subscriptionId" 1> /dev/null \
            && is-guid ".initConfig.tenantId" 1> /dev/null \
            && is-valid-location ".initConfig.location" 1> /dev/null \
            && value-exist ".initConfig.naming.solutionName" 1> /dev/null \
            && value-exist ".initConfig.naming.solutionPrefix" 1> /dev/null \
            && is-valid-b2c-location ".initConfig.azureb2c.location" 1> /dev/null \
            && value-exist ".initConfig.azureb2c.countryCode" 1> /dev/null \
            && is-valid-b2c-sku ".initConfig.azureb2c.skuName" 1> /dev/null \
            && is-valid-b2c-tier ".initConfig.azureb2c.tier" 1> /dev/null \
            && echo "All required initial configuration settings exist." | log-output --level success
        
        return 

    ) || 
    ( 
        echo "One or more required initial configuration settings are missing or incorrect." \
        | log-output \
            --level error \
            --header "Configuration Missing" 

        init_config="$( get-value ".initConfig" )"

        echo "$init_config";
        
        exit 1
    )
}

function initialize() {

    if [[ ! -f "${CONFIG_FILE}" ]]; then
        echo "Welcome to the Azure SaaS Dev Kit - Azure B2C Identity Provider deployment script." \
            | log-output \
                --level info \
                --header "Welcome"

        echo

        echo "It looks like this is the first time you're running this script. Setting things up..." \
            | log-output --level info

        echo

        echo "Creating './config/config.json' from 'config-template.json.'" \
            | log-output --level info
        
        cp "${BASEDIR}/config/config-template.json" "${CONFIG_FILE}"

        echo

        echo "Before beginning deployment you must specify initial configuration in the 'initConfig' object:" \
            | log-output \
                --level warning

        init_config="$( get-value ".initConfig" )"

        echo "${init_config}" | log-output --level msg;

        echo

        echo "Please add initial settings to the initConfig object in ./config/config.json and run this script again." \
            | log-output \
                --level warning
        exit 0
    else

        # Setting configuration variables
        echo "Initializing Configuration" | log-output -l info -h "Configation Settings"

        # get now date and time for backup file name
        now=$(date '+%Y-%m-%d--%H-%M-%S')
        backup_file="${CONFIG_FILE}.${now}.bak"

        echo "Backing up existing configuration file to: ${backup_file}" | log-output --level info
        cp "${CONFIG_FILE}" "${backup_file}"
    fi

    echo "Working in directory ${BASEDIR}." \
    | log-output \
        --level info \
        --header "Configuration Files"

    echo "Configuration settings: $CONFIG_FILE." | log-output --level success

    echo "Please log in with sudo to run this script." \
    | log-output \
        --level info \
        --header "Sudo"

    # ensure scripts are executable
    sudo chmod +x ./*.sh
    echo "You are logged in with sudo." \
        | echo-color \
            --level success

    config-file-exists-or-exit

    postfix="$( get-value ".deployment.postfix" )"

    # Only add new random postfix if it doesn't already exist in /config/config.json.
    # To force the creating of a new RAND delete the field in /config/config.json.
    if [[ -z ${postfix} || ${postfix} == "null" ]]; then
        postfix="$( LC_CTYPE=C tr -dc 'a-z0-9' < /dev/urandom | fold -w 4 | head -n 1 )"
        echo "Created new postfix: ${postfix}" | log-output \
            --level info \
            --header "Postfix" 

        echo "The unique postfix ${postfix} will be used for naming resources." \
            | log-output --level info

        echo "As long as the postfix is unchanged, any rerun of this script will continue were it left off." \
            | log-output --level info

        echo "If the post fix is deleted or changed, an all new deployment will be created when rerunning this script." \
            | log-output --level warning

        put-value ".deployment.postfix" "${postfix}"
    else
        echo "Using existing postfix to continue or patch existing deployment: ${postfix}" \
            | log-output \
                --level info\
            --header "Postfix" 
    fi

    check-settings || exit 1

    solution_name=$( get-value ".initConfig.naming.solutionName ")
    solution_prefix=$( get-value ".initConfig.naming.solutionPrefix ")

    full_solution_name=${solution_prefix}-${solution_name}-${postfix}

    # defining resource group name setting
    put-value '.deployment.resourceGroup.name'  \
        "rg-${solution_prefix}-${solution_name}-${postfix}"

    b2c_display_name="b2c-${full_solution_name}"
    put-value ".deployment.azureb2c.displayName" "${b2c_display_name}"

    b2c_name="$( sed -E 's/[^[:alnum:]]//g' <<< "${solution_prefix}${solution_name}${postfix}" )"
    put-value ".deployment.azureb2c.name" "${b2c_name}.onmicrosoft.com"

    put-value ".deployment.keyVault.name" "kv-${full_solution_name}"
    put-value ".deployment.keyVault.key.name" "key-${full_solution_name}"

    echo "Creating two temporary local linux users for use during the deployment. Both users and any data stored in the users home directory will be deleted if the script fails or after the deployment script and complete." \
        | log-output \
            --level info \
            --header "Linux Users"

    b2c_config_usr_name="${solution_prefix}-usr-b2c-${postfix}"
    put-value ".deployment.azureb2c.username" "${b2c_config_usr_name}"

    b2c_config_usr_home_dir="/home/${b2c_config_usr_name}"
    put-value ".deployment.azureb2c.userHomeDir" "${b2c_config_usr_home_dir}"

    create-linux-user "${b2c_config_usr_name}" "${b2c_config_usr_home_dir}" "certs"
    echo "User ${b2c_config_usr_name} have been created." | log-output --level success

    put-value ".deployment.azureb2c.certificateDir" "${b2c_config_usr_home_dir}/certs"

    service_principal_name="${solution_prefix}-usr-sp-${postfix}"
    put-value ".deployment.azureb2c.servicePrincipal.username" "${service_principal_name}"

    service_principal_home_dir="/home/${service_principal_name}"
    put-value ".deployment.azureb2c.servicePrincipal.userHomeDir" "${service_principal_home_dir}"

    create-linux-user "${service_principal_name}" "${service_principal_home_dir}" "secrets"
    echo "User ${service_principal_name} have been created." | log-output --level success

    put-value ".deployment.azureb2c.servicePrincipal.secretDir" "${service_principal_home_dir}/secrets"
}
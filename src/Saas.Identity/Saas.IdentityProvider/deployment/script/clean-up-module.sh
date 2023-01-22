#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
source "$SHARED_MODULE_DIR/config-module.sh"
source "$SHARED_MODULE_DIR/log-module.sh"
source "$SHARED_MODULE_DIR/util-module.sh"
source "$SHARED_MODULE_DIR/user-module.sh"
source "$SHARED_MODULE_DIR/service-principal-module.sh"
source "$SHARED_MODULE_DIR/backup-module.sh"

function clean-up-after-service-principal() {

    # deleting the service principal credentials file
    echo "Deleting locally stored service principal credentials." \
        | log-output \
            --level info

    service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
    service_principal_credentials_file_path="$( get-user-value "${service_principal_username}" "credentialsPath" )"
    sudo rm -f "${service_principal_credentials_file_path}"

    # deleting service principal credentials in Azure AD too
    app_id="$( get-value ".deployment.azureb2c.servicePrincipal.appId" )"
    b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
    echo "Deleting service principal credentials using user '${b2c_config_usr_name}'" \
        | log-output \
            --level info

    # setting user context to the user that will be used for configuring Azure B2C
    set-user-context "${b2c_config_usr_name}"

    # deleting the service principal
    delete-service-principal-credentials "${app_id}"

    # resetting user context to the user that was used to login to the tenant
    reset-user-context

    echo "Service principal credentials have been removed locally and in Azure AD." \
        | log-output \
            --level success
}

function clean-up() {
    
    set  +e +o pipefail

    backup-config-end

    # if settings doesn't exist then the script didn't get far and in any case we're not able to clean up when we don't have the settings.
    check-settings > /dev/null || exit 1

    echo "Starting cleaning up..." \
        | log-output \
            --level info \
            --header "Cleaning up"

    echo 

    clean-up-after-service-principal

    # deleting the temp service principal user as we no longer need it.
    service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
    delete-user-context-data "${service_principal_username}"
    echo "User context for ${service_principal_username} have been deleted." | log-output --level success

    # Deleting the temp b2c configuration user as we no longer need it.
    delete-user-context-data "${b2c_config_usr_name}"
    echo "User context for ${b2c_config_usr_name} have been deleted." | log-output --level success
    echo "Clean up has completed." \
        | log-output \
            --level success

    backup-log "${SCRIPT_NAME}"

    set -e -o pipefail

    final-state 
}
#!/usr/bin/env bash

set -u -e -o pipefail

# include script modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/colors-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
source "$SCRIPT_MODULE_DIR/user-module.sh"

# setting user context to the user that will be used to configure Azure B2C
b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
set-user-context "${b2c_config_usr_name}"

# run the shell script for provisioning the Azure B2C app registrations
"${SCRIPT_DIR}/b2c-app-registrations.sh" \
    || echo "Azure B2C app registrations failed." \
        | log-output \
            --level Error \
            --header "Critical Error" \
            || exit 1

echo "Azure B2C app registrations have completed." \
        | log-output \
            --level success \

# run the shell script for provioning the Azure B2C service principal needed to configure the Azure B2C policies
"${SCRIPT_DIR}/create-service-principal.sh" \
    || echo "Service principal creation/update failed." \
        | log-output \
            --level Error \
            --header "Critical Error" \
            || exit 1

echo "Service principal update/creation completed." \
        | log-output \
            --level success \

# resetting user context to the default User
reset-user-context

# set the user context to the service principal to run shell script to configure the Azure B2C policy keys
service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
set-user-context "${service_principal_username}"

# run shell script for provisioning the Azure B2C policy keys 
"${SCRIPT_DIR}/b2c-policy-keys.sh" \
    || echo "B2C policy configuration script failed." \
        | log-output \
            --level error \
            --header "Critical Error" \
            || exit 1

echo "B2C policy configuration script has completed." \
    | log-output \
        --level success

# resetting user context to the default User
reset-user-context
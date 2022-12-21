#!/bin/bash
set -u -e -o pipefail

# include script modules into current shell
source "./script/config-module.sh"
source "./script/colors-module.sh"
source "./script/linux-user-module.sh"
source "./script/log-module.sh"

b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"

sudo runuser -u "${b2c_config_usr_name}" "./shell-b2c-app-registrations.sh" \
    || echo "Azure B2C app registrations failed." \
        | log-output \
            --level Error \
            --header "Critical Error" \
            | exit 1

echo "Azure B2C app registration completed." \
        | log-output \
            --level success \

sudo runuser -u "${b2c_config_usr_name}" "./shell-create-service-principal.sh" \
    || echo "Service principal creation/update failed." \
        | log-output \
            --level Error \
            --header "Critical Error" \
            | exit 1

echo "Service principal update/creation completed." \
        | log-output \
            --level success \

# get the path to the service principal credentials file
credentials_path="$( get-value ".deployment.azureb2c.servicePrincipal.credentialsPath" )"

# certificate path to the home directory of the service principal linux user.
service_principal_home_dir="$( get-value ".deployment.azureb2c.servicePrincipal.userHomeDir" )"

# service principal username
service_principal_username="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"

new_path="$( move-file-to-home-dir-of-user "${credentials_path}" "${service_principal_home_dir}" "${service_principal_username}" )"

# store revised certificate path in config
put-value ".deployment.azureb2c.servicePrincipal.credentialsPath" "${new_path}"

sudo runuser -u "${service_principal_username}" "./shell-b2c-policy-keys.sh" \
    || echo "B2C policy configuration script failed." \
        | log-output \
            --level error \
            --header "Critical Error" \
            | exit 1

echo "B2C policy configuration script has completed." | log-output --level success
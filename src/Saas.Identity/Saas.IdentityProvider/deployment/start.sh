#!/usr/bin/env bash

NC='\033[0m' # No Color
YELLOW='\033[1;33m'

ASDK_ID_PROVIDER_DEPLOYMENT_VERSION="0.8.0"

if [[ -z $ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR ]] ; then
    base_dir="$( dirname "$( readlink -f "$0" )" )"
    echo -e "${YELLOW}ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR is not set".
    echo -e "Setting it to current root: ${base_dir}${NC}"
    export ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR=$base_dir
fi

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

# include script modules into current shell
source "./constants.sh"
source "$SCRIPT_MODULE_DIR/colors-module.sh"
source "$SCRIPT_MODULE_DIR/init-module.sh"
source "$SCRIPT_MODULE_DIR/util-module.sh"
source "$SCRIPT_MODULE_DIR/tenant-login-module.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
source "$SCRIPT_MODULE_DIR/resource-module.sh"
source "$SCRIPT_MODULE_DIR/clean-up-module.sh"

# get now date and time for backup file name
now=$(date '+%Y-%m-%d--%H-%M-%S')

# set run time for deployment script instance
export ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME="${now}"

# create log file directory if it does not exist
if [[ ! -d "${LOG_FILE_DIR}" ]]; then
    mkdir -p "$LOG_FILE_DIR"
    sudo chown -R 666 "$LOG_FILE_DIR"
fi

# create log file for this deployment script instance
touch "${LOG_FILE_DIR}/deploy-${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}.log"

echo "Welcome to the Azure SaaS Dev Kit - Azure B2C Identity Provider deployment script." \
    | log-output \
        --level info \
        --header "Welcome"

echo "Working in directory ${BASE_DIR}." \
    | log-output \
        --level info \
        --header "Preparing"

echo "Please log in with sudo to run this script." \
| log-output \
    --level info \
    --header "Sudo"

# clear sudo timer  
sudo -K

# log in with sudo
sudo echo "You are logged in with sudo." \
    | echo-color \
        --level success

# make sure that the shell-init script is executable
chmod +x "$SCRIPT_DIR/shell-init.sh"

# initialize deployment environment
"${SCRIPT_DIR}/shell-init.sh" \
    || if [[ $? -eq 2 ]]; then exit 0; fi

# from hereon and on run clean-up script on exit, interrupt and terminination of script
trap clean-up EXIT INT TERM

(
    # check to see if the postfix exists. If so use it, if not create it.
    initialize-post-fix

    # check to see if all initial configuration values exist in config.json, exit if not.
    check-settings

    # Log in to you tenant, if you are not already logged in
    log-in-to-main-tenant

    # check to see if settings are valid 
    continue-validating-configuration-settings

    # populate the configuration manifest with additional setting needed for deployment
    populate-configuration-manifest

    # preserve the Azure CLI context so that it can be restored after the deployment
    preserve-azure-cli-context

    # create user context for Azure B2C user and Azure B2C service principal
    intialize-context-for-automation-users    
) \
    || echo "Initilization failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

# service message
echo "Please don't go anywhere. You will be required to log into the Azure B2C tenant in a few minutes. Thank you." \
    | log-output \
        --level warning \
        --header "One more login request is coming up." \

# Creating resource group if it does not already exist
resource_group="$( get-value ".deployment.resourceGroup.name" )"
location="$( get-value ".initConfig.location" )"

echo "Provisioning resource group ${resource_group}..." | log-output --level info --header "Resource Group"  
( create-resource-group "${resource_group}" "${location}" \
    && put-value ".deployment.resourceGroup.provisionState" "successful" ) \
    || echo "Creation of resource group failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1
 
# Creating Azure Key Vault if it does not already exist
 ( "${SCRIPT_DIR}/create-key-vault.sh" \
    && put-value ".deployment.keyVault.provisionState" "successful" ) \
    || echo "Creation of KeyVault failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

echo "Provisioning Azure B2C..." | log-output --level info --header "Azure B2C Tenant"  
# Creating Azure AD B2C Directory if it does not already exist
( "${SCRIPT_DIR}/create-azure-b2c.sh" \
    && put-value ".deployment.azureb2c.provisionState" "successful" ) \
    || echo "Creation of Azure B2C tenant failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

# Configuring Azure the AD B2C Tenant
( "${SCRIPT_DIR}/config-b2c.sh" \
    && put-value ".deployment.azureb2c.configurationState" "successful" ) \
    || echo "Configuration of Azure B2C tenant failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1


# Deploying Identity Provider
( "${SCRIPT_DIR}/deploy-identity-provider.sh" \
    && put-value ".deployment.identityProvider.provisionState" "successful" ) \
    || echo "Deployment of Identity Provider failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

# Uploading IEF custom policies
( "${SCRIPT_DIR}/upload-ief-policies.sh" \
    && put-value ".deployment.iefPolicies.provisionState" "successful" ) \
    || echo "Upload of IEF policies failed." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1


# end of script - which means running the clean-up script before exiting
exit 0

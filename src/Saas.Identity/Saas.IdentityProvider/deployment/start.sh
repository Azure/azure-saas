#!/bin/bash

set -u -e -o pipefail

# include script modules into current shell
source "./script/colors-module.sh"
source "./script/init-module.sh"
source "./script/tenant-login-module.sh"
source "./script/config-module.sh"
source "./script/log-module.sh"
source "./script/resource-module.sh"
source "./script/constants-module.sh"

clear

if [[ ! -d "${LOG_FILE_DIR}" ]]; then
    mkdir -p "$LOG_FILE_DIR"
fi

# reset log
rm -f ./log/deploy.log


echo "This deployment script have been test for az cli version 2.42.0 and higher." \
    | log-output \
        --header "Deployment script" \
        --level info

# set to install az cli extensions without prompting
az config set extension.use_dynamic_install=yes_without_prompt &> /dev/null

echo "You are running az cli $(az version | jq '."azure-cli"' )" \
    | log-output \
        --level info

initialize \
    || echo "Initilization failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            | exit 1

# clean up on exit, interrupt and terminination of script
trap clean-up EXIT INT TERM

subscription_id="$( get-value ".initConfig.subscriptionId" )"
tenant_id="$( get-value ".initConfig.tenantId" )"

# Log in to you tenant, if you are not already logged in
echo "Log into you Azure tenant" | log-output --level info --header "Login to Azure"  
log-into-main "${subscription_id}" "${tenant_id}"

echo "You will be required to log into the Azure B2C tenant in a few minutes. Thank you." \
    | log-output \
        --level warning \
        --header "One more login request in coming up" \

# Creating resource group
resource_group="$( get-value ".deployment.resourceGroup.name" )"
location="$( get-value ".initConfig.location" )"

echo "Provisioning resource group ${resource_group}..." | log-output --level info --header "Resource Group"  
( create-resource-group "${resource_group}" "${location}" \
    && put-value ".deployment.resourceGroup.provisionState" "successful" ) \
    || echo "Creation of resource group failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            | exit 1

# Creating Azure Key Vault
 ( ./create-Key-vault.sh \
    && put-value ".deployment.keyVault.provisionState" "successful" ) \
    || echo "Creation of KeyVault failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            | exit 1

echo "Provisioning Azure B2C..." | log-output --level info --header "Azure B2C Tenant"  
# Creating Azure AD B2C Directory
( ./create-azure-b2c.sh \
    && put-value ".deployment.azureb2c.provisionState" "successful" ) \
    || echo "Creation of Azure B2C tenant failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            | exit 1

# Configuring Azure the AD B2C Tenant
( ./config-b2c.sh \
    && put-value ".deployment.azureB2C.configurationState" "successful" ) \
    || echo "Configuration of Azure B2C tenant failed" \
        | log-output \
            --level error \
            --header "Critical error" \
            | exit 1


#!/usr/bin/env bash

# include script modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/colors-module.sh"
source "$SCRIPT_MODULE_DIR/tenant-login-module.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
source "$SCRIPT_MODULE_DIR/util-module.sh"
source "$SCRIPT_MODULE_DIR/user-module.sh"
source "$SCRIPT_MODULE_DIR/service-principal-module.sh"

function final-state() {
    resource_group_state="$( get-value ".deployment.resourceGroup.provisionState" )"
    key_vault_state="$( get-value ".deployment.keyVault.provisionState" )"
    azure_b2c_state="$( get-value ".deployment.azureb2c.provisionState" )"
    azure_b2c_config_state="$( get-value ".deployment.azureb2c.configurationState" )"

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

function check-settings() {
    # check to see if all initial configuration values exist in config.json, exit if not.
    echo "Validating Initial Configuration Settings..." \
        | log-output \
            --level info \
            --header "Configuration Validation"

    ( 
         is-guid ".initConfig.subscriptionId" 1> /dev/null \
            && is-guid ".initConfig.tenantId" 1> /dev/null \
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

function initialize-post-fix() {
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
}

function log-in-to-main-tenant() {
    subscription_id="$( get-value ".initConfig.subscriptionId" )"
    tenant_id="$( get-value ".initConfig.tenantId" )"

    # Log in to you tenant, if you are not already logged in
    echo "Log into you Azure tenant" | log-output --level info --header "Login to Azure"  
    log-into-main "${tenant_id}" "${subscription_id}"
}

function continue-validating-configuration-settings() {
    location_value=$( get-value ".initConfig.location" )
    
    # check location settings for resource group
    is-valid-location ".initConfig.location" 1> /dev/null || \
        echo "The value '${location_value}' of '.initConfig.location' is not a valid location." \
            | log-output \
                --level error \
                --header "Critical error"
}

function populate-configuration-manifest() {

    set-version "${ASDK_ID_PROVIDER_DEPLOYMENT_VERSION}"

    dev_machine_ip="$( dig +short myip.opendns.com @resolver1.opendns.com )"

    put-value '.deployment.devMachine.ip' "${dev_machine_ip}"

    # defining solution name setting
    solution_name=$( get-value ".initConfig.naming.solutionName ")
    solution_prefix=$( get-value ".initConfig.naming.solutionPrefix ")
    long_solution_name="${solution_prefix}-${solution_name}-${postfix}"

    # defining resource group name setting
    put-value '.deployment.resourceGroup.name'  \
        "rg-${long_solution_name}"

    # defining Azure B2C display name
    b2c_display_name="b2c-${long_solution_name}"
    put-value ".deployment.azureb2c.displayName" "${b2c_display_name}"

    # defining Azure B2C name
    b2c_name="$( sed -E 's/[^[:alnum:]]//g' <<< "${solution_prefix}${solution_name}${postfix}" )"
    put-value ".deployment.azureb2c.domainName" "${b2c_name}.onmicrosoft.com"
    put-value ".deployment.azureb2c.name" "${b2c_name}"

    # defining Azure B2C key vault name
    put-value ".deployment.keyVault.name" "kv-${long_solution_name}"

    # defining Azure B2C key vault key name
    put-value ".deployment.keyVault.key.name" "key-${long_solution_name}"

    # defining Azure B2C user name
    b2c_config_usr_name="${solution_prefix}-usr-b2c-${postfix}"
    put-value ".deployment.azureb2c.username" "${b2c_config_usr_name}"

    # defining Azure B2C service principal name
    service_principal_name="${solution_prefix}-usr-sp-${postfix}"
    put-value ".deployment.azureb2c.servicePrincipal.username" "${service_principal_name}"

    put-app-value \
        "admin-api" \
        "baseUrl" \
        "api-admin-${long_solution_name}"

    # adding redirecturl to signupadmin-app
    put-app-value \
        "signupadmin-app" \
        "redirectUri" \
        "https://appsignup-${long_solution_name}.azurewebsites.net/signin-oidc"

    # adding redirecturl to saas-app
    put-app-value \
        "saas-app" \
        "redirectUri" \
        "https://saasapp-${long_solution_name}.azurewebsites.net/signin-oidc"

    permission_api_name="api-permission-${long_solution_name}"

    # adding apiName to permissions-api
    put-app-value \
        "permissions-api" \
        "apiName" \
        "${permission_api_name}"

    # adding permission API Url to permissions-api
    put-app-value \
        "permissions-api" \
        "permissionsApiUrl" \
        "https://${permission_api_name}.azurewebsites.net/api/CustomClaims/permissions"

    # adding roles API Url to permissions-api
    put-app-value \
        "permissions-api" \
        "rolesApiUrl" \
        "https://${permission_api_name}.azurewebsites.net/api/CustomClaims/roles"
    

}

function intialize-context-for-automation-users() {

    # create user context for Azure B2C user
    b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
    create-user-context "${b2c_config_usr_name}" "${CERTIFICATE_DIR_NAME}" "certs"
    echo "User context for '${b2c_config_usr_name}' have been created." | log-output --level success

    # create user context for Azure B2C service principal
    service_principal_name="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
    create-user-context "${service_principal_name}" "${SECRET_DIR_NAME}" "secrets"
    echo "User context for '${service_principal_name}' have been created." | log-output --level success
}
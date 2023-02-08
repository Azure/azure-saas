#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
{
    source "$SCRIPT_DIR/post-fix-module.sh"
    source "$SHARED_MODULE_DIR/colors-module.sh"
    source "$SHARED_MODULE_DIR/tenant-login-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/util-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
    source "$SHARED_MODULE_DIR/service-principal-module.sh"
}

function final-state() {
    resource_group_state="$( get-value ".deployment.resourceGroup.provisionState" )"
    storage_state="$( get-value ".deployment.storage.provisionState" )"
    key_vault_state="$( get-value ".deployment.keyVault.provisionState" )"
    azure_b2c_state="$( get-value ".deployment.azureb2c.provisionState" )"
    azure_b2c_config_state="$( get-value ".deployment.azureb2c.configuration.provisionState" )"
    identity_foundation_state="$( get-value ".deployment.identityFoundation.provisionState" )"
    ief_policies_state="$( get-value ".deployment.iefPolicies.provisionState" )"
    oidc_state="$( get-value ".deployment.oidc.provisionState" )"

    if [[ "${resource_group_state}" == "successful" ]] \
        && [[ "${storage_state}" == "successful" ]] \
        && [[ "${key_vault_state}" == "successful" ]] \
        && [[ "${azure_b2c_state}" == "successful" ]] \
        && [[ "${azure_b2c_config_state}" == "successful" ]] \
        && [[ "${identity_foundation_state}" == "successful" ]] \
        && [[ "${ief_policies_state}" == "successful" ]] \
        && [[ "${oidc_state}" == "successful" ]]; then
        echo "Identity Foundation deployed successfully." \
            | log-output \
                --level success \
                --header "Deployment script completion"

        resource_group="$( get-value ".deployment.resourceGroup.name" )"
        subscription_id="$( get-value ".initConfig.subscriptionId" )"

        echo "To see the deployed resources, please visit the Azure Portal and navigate to the resource group '${resource_group}': https://ms.portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/${subscription_id}/resourceGroups/${resource_group}/overview" \
            | log-output \
                --level info

        b2c_tenant_id="$( get-value ".deployment.azureb2c.tenantId" )"

        echo "Your Azure B2C Tenant Id is: ${b2c_tenant_id}" \
            | log-output \
                --level info

        echo "Link to visit your Azure B2C tenant: https://portal.azure.com/${b2c_tenant_id}" \
            | log-output \
                --level info

    else
        echo "Identity Foundation deployed with errors." \
            | log-output \
                --level error \
                --header "Deployment script completion" \
            || echo "Please review the log file for more details: ${LOG_FILE_DIR}/${ASDK_DEPLOYMENT_SCRIPT_RUN_TIME}" \
                | log-output \
                    --level warn
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

    # only add new random postfix if it doesn't already exist in /config/config.json.
    if [[ -z ${postfix} || ${postfix} == "null" ]]; then

        echo "Creating new postfix: ${postfix}" \
            | log-output \
                --level info \
                --header "Postfix" 

        # create a postfix to be used for naming resources.
        # if you prefer another naming convention than a random string of four characters
        # you can change this function to return a different value.
        postfix="$( get-postfix )"
    
        echo "The unique postfix '${postfix}' will be used for naming resources." \
            | log-output \
                --level info

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
    echo "Log into you Azure tenant" \
        | log-output \
            --level info \
            --header "Login to Azure"  
    
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

    # defining solution name setting
    solution_name="$( get-value ".initConfig.naming.solutionName" | cut -c 1-16 )"
    solution_prefix="$( get-value ".initConfig.naming.solutionPrefix" | cut -c 1-6 )"
    long_solution_name="${solution_prefix}-${solution_name}-${postfix}"

    # getting public ip address for user, for use in database firewall rules
    dev_machine_ip="$( dig +short myip.opendns.com @resolver1.opendns.com )" \
        || echo "Unable to determine your public IP address." \
            | log-output \
                --level error \
                --header "Critical error" \
                || exit 1

    put-value ".deployment.devMachine.ip" "${dev_machine_ip}"

    # defining storage account name 3-24 characters, only lowercase letters and numbers
    storage_account_name="$( sed -E 's/[^[:alnum:]]//g;s/[A-Z]/\L&/g' \
        <<< "st${solution_prefix}${solution_name}${postfix}" \
        | cut -c 1-24 )"

    put-value ".deployment.storage.name" "${storage_account_name}"

    storage_container_name="blob-${long_solution_name}"
    put-value ".deployment.storage.containerName" "${storage_container_name}"


    if [ -f /.dockerenv ]; then
        set +u
        if [ -z "${GIT_REPO_ORIGIN}" ]; then
            echo "GIT_REPO_ORIGIN is not set for container. Before running the script again, please set the environment variable using the command: 'export GIT_REPO_ORIGIN=\"$( git config --get remote.origin.url )\"'." \
                | log-output \
                    --level error \
                    --header "Critical error" \
                    || exit 1
        fi

        if [ -z "${GIT_ORG_PROJECT_NAME}" ]; then
            echo "GIT_ORG_PROJECT_NAME is not set for container. Before running the script again, please set the environment variable using the command: 'export GIT_ORG_PROJECT_NAME=\"$( git config --get remote.origin.url | sed 's/.*\/\([^ ]*\/[^.]*\).*/\1/' )\"'." \
                | log-output \
                    --level error \
                    --header "Critical error" \
                    || exit 1
        fi
        set -u

        put-value ".git.repo" "${GIT_REPO_ORIGIN}"
        put-value ".git.orgProjectName" "${GIT_ORG_PROJECT_NAME}"
    else
        git_repo_origin="$( git config \
            --get remote.origin.url )"

        git_org_project_name="$( git config \
            --get remote.origin.url \
            | sed 's/.*\/\([^ ]*\/[^.]*\).*/\1/' )"
        
        put-value ".git.repo" "${git_repo_origin}"
        put-value ".git.orgProjectName" "${git_org_project_name}"
    fi

    # For more about OIDC Workflows see: https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=openid
    put-value ".oidc.name" "oidc-workflow-${long_solution_name}"
    put-value ".oidc.credentials.name" "oidc-credential-${long_solution_name}"

    # defining resource group name setting
    put-value ".deployment.resourceGroup.name" "rg-${long_solution_name}"

    # defining Azure B2C display name
    b2c_display_name="b2c-${long_solution_name}"
    put-value ".deployment.azureb2c.displayName" "${b2c_display_name}"

    # defining Azure B2C name only lowercase letters and numbers, and no more than 43 characters
    b2c_name="$( sed -E 's/[^[:alnum:]]//g;s/[A-Z]/\L&/g' \
        <<< "${solution_prefix}${solution_name}${postfix}" \
        | cut -c 1-43 )"

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

    admin_api_name="admin-api-${long_solution_name}"

    put-app-value \
        "admin-api" \
        "appServiceName" \
        "${admin_api_name}"

    put-app-value \
        "admin-api" \
        "baseUrl" \
        "https://${admin_api_name}.azurewebsites.net"

    put-app-value \
        "admin-api" \
        "applicationIdUri" \
        "https://${b2c_name}.onmicrosoft.com/admin-api"

    signup_admin_app_name="signupadmin-app-${long_solution_name}"

    put-app-value \
        "signupadmin-app" \
        "appServiceName" \
        "${signup_admin_app_name}"

    # adding redirecturl to signupadmin-app
    put-app-value \
        "signupadmin-app" \
        "redirectUri" \
        "https://signupadmin-app-${long_solution_name}.azurewebsites.net/signin-oidc"

    saas_app_name="saas-app-${long_solution_name}"

    put-app-value \
        "saas-app" \
        "appServiceName" \
        "${saas_app_name}"

    # adding redirecturl to saas-app
    put-app-value \
        "saas-app" \
        "redirectUri" \
        "https://saas-app-${long_solution_name}.azurewebsites.net/signin-oidc"

    permission_api_name="api-permission-${long_solution_name}"

    # adding apiName to permissions-api
    put-app-value \
        "permissions-api" \
        "apiName" \
        "${permission_api_name}"

    put-app-value \
        "permissions-api" \
        "appServiceName" \
        "${permission_api_name}"

    put-app-value \
        "permissions-api" \
        "baseUrl" \
        "https://${permission_api_name}.azurewebsites.net"

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

    # adding redirecturl to IdentityExperienceFramework
    put-app-value \
        "IdentityExperienceFramework" \
        "redirectUri" \
        "https://${b2c_name}.b2clogin.com/${b2c_name}.onmicrosoft.com"

    put-app-value \
        "IdentityExperienceFramework" \
        "applicationIdUri" \
        "api://${b2c_name}/${long_solution_name}/identityexperienceframework"
}

function intialize-context-for-automation-users() {

    # create user context for Azure B2C user
    b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
    create-user-context "${b2c_config_usr_name}" "${CERTIFICATE_DIR_NAME}" "certs" true
    echo "User context for '${b2c_config_usr_name}' have been created." \
        | log-output \
            --level success

    # create user context for Azure B2C service principal
    service_principal_name="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
    create-user-context "${service_principal_name}" "${SECRET_DIR_NAME}" "secrets" false
    echo "User context for '${service_principal_name}' have been created." \
        | log-output \
            --level success
}
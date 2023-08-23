#!/usr/bin/env bash

export ASDK_CACHE_AZ_CLI_SESSIONS=true

# work-around for issue w/ az cli bicep v. 2.46: https://github.com/Azure/azure-cli/issues/25710
# az config set bicep.use_binary_from_path=false

# if not running in a container
if ! [ -f /.dockerenv ]; then
    echo "Running outside of a container us not supported. Please run the deployment script using './run.sh'."
    exit 0
fi

if [[ -z $ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE ]]; then
    # repo base
    echo "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE is not set. Setting it to default value."
    repo_base="$(git rev-parse --show-toplevel)"
    project_base="${repo_base}/src/Saas.Identity/Saas.IdentityProvider/deployment"
    export ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE="${project_base}"
fi

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SCRIPT_DIR/init-module.sh"
    source "$SCRIPT_DIR/clean-up-module.sh"
    source "$SHARED_MODULE_DIR/util-module.sh"
    source "$SHARED_MODULE_DIR/tenant-login-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/resource-module.sh"
    source "$SHARED_MODULE_DIR/storage-module.sh"
}

# get now date and time for backup file name
now=$(date '+%Y-%m-%d--%H-%M-%S')

# set run time for deployment script instance
export ASDK_DEPLOYMENT_SCRIPT_RUN_TIME="${now}"

# create log file directory if it does not exist
if ! [ -f /.dockerenv ] && [[ ! -d "${LOG_FILE_DIR}" ]]; then
    mkdir -p "$LOG_FILE_DIR"
    sudo chown -R 666 "$LOG_FILE_DIR"
fi

# create log file for this deployment script instance
touch "${LOG_FILE_DIR}/deploy-${ASDK_DEPLOYMENT_SCRIPT_RUN_TIME}.log"

echo "Welcome to the Azure SaaS Dev Kit - Azure B2C Identity Provider deployment script." |
    log-output \
        --level info \
        --header "Welcome"

echo "Working in directory ${BASE_DIR}." |
    log-output \
        --level info \
        --header "Preparing"

echo "Please log in with sudo to run this script." |
    log-output \
        --level info \
        --header "Sudo"

# clear sudo timer
sudo -K

# log in with sudo
sudo echo "You are logged in with sudo." |
    echo-color \
        --level success

# initialize deployment environment
"${SCRIPT_DIR}/init.sh" ||
    if [[ $? -eq 2 ]]; then exit 0; fi

# from hereon and on run clean-up script on exit, interrupt and terminination of script
trap clean-up EXIT INT TERM

(
    # check to see if the postfix exists. If so use it, if not create it.
    initialize-post-fix ||
        echo "Initialization of postfix failed." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # check to see if all initial configuration values exist in config.json, exit if not.
    check-settings ||
        echo "Configuration settings are missing." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # Log in to you tenant, if you are not already logged in
    log-in-to-main-tenant ||
        echo "Log in to tenant failed." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # check to see if settings are valid
    continue-validating-configuration-settings ||
        echo "Validation of configuration settings failed." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # populate the configuration manifest with additional setting needed for deployment
    populate-configuration-manifest ||
        echo "Populating configuration manifest failed." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # preserve the Azure CLI context so that it can be restored after the deployment
    preserve-azure-cli-context ||
        echo "Preserving Azure CLI context failed." |
        log-output \
            --level error \
            --header "Critical error" ||
        exit 1

    # create user context for Azure B2C user and Azure B2C service principal
    intialize-context-for-automation-users
) || echo "Initilization failed" |
    log-output \
        --level error \
        --header "Critical error" ||
    exit 1

# service message
echo "Please don't go anywhere just yet. You may need to log into the Azure B2C tenant in a few minutes. Thank you." |
    log-output \
        --level warning \
        --header "Attention."

# Creating resource group if it does not already exist
resource_group="$(get-value ".deployment.resourceGroup.name")"
location="$(get-value ".initConfig.location")"

echo "Provisioning resource group ${resource_group}..." |
    log-output \
        --level info \
        --header "Resource Group"

put-value ".deployment.resourceGroup.provisionState" "provisioning"
(
    create-resource-group "${resource_group}" "${location}"
    put-value ".deployment.resourceGroup.provisionState" "successful"
) ||
    (
        put-value ".deployment.resourceGroup.provisionState" "failed" &&
            echo "Creation of resource group failed." |
            log-output \
                --level error \
                --header "Critical error"

    ) || exit 1

# Creating storage it does not already exist
put-value ".deployment.storage.provisionState" "provisioning"
(
    storage_account_name="$(get-value ".deployment.storage.name")"

    if ! storage-account-exist "${resource_group}" "${storage_account_name}"; then

        storage_container_name="$(get-value ".deployment.storage.containerName")"
        subscription_id="$(get-value ".initConfig.subscriptionId")"
        user_principal_id="$(get-value ".initConfig.userPrincipalId")"

        storage-create-bicep \
            "${resource_group}" \
            "${storage_account_name}" \
            "${location}" \
            "${storage_container_name}" \
            "${subscription_id}" \
            "${user_principal_id}" ||
            echo "Storage creation failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi
    put-value ".deployment.storage.provisionState" "successful"
) ||
    (
        put-value ".deployment.storage.provisionState" "failed" &&
            echo "Storage failed exists." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

put-value ".deployment.keyVault.provisionState" "provioning"
# Creating Azure Key Vault if it does not already exist
(
    "${SCRIPT_DIR}/create-key-vault.sh" &&
        put-value ".deployment.keyVault.provisionState" "successful"
) ||
    (
        put-value ".deployment.keyVault.provisionState" "failed" &&
            echo "Creation of KeyVault failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

echo "Provisioning Azure B2C..." |
    log-output \
        --level info \
        --header "Azure B2C Tenant"

put-value ".deployment.azureb2c.provisionState" "provisioning"
# Creating Azure AD B2C Directory if it does not already exist
(
    "${SCRIPT_DIR}/create-azure-b2c.sh"
    put-value ".deployment.azureb2c.provisionState" "successful"
) ||
    (
        put-value ".deployment.azureb2c.provisionState" "failed" &&
            echo "Creation of Azure B2C tenant failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

put-value ".deployment.azureb2c.configuration.provisionState" "provisioning"
# Configuring Azure the AD B2C Tenant
(
    "${SCRIPT_DIR}/config-b2c.sh" &&
        put-value ".deployment.azureb2c.configuration.provisionState" "successful"
) ||
    (
        put-value ".deployment.azureb2c.configuration.provisionState" "failed" &&
            echo "Configuration of Azure B2C tenant failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

put-value ".deployment.identityFoundation.provisionState" "provisioning"
# Deploying Identity Provider
(
    "${SCRIPT_DIR}/deploy-identity-foundation.sh" &&
        put-value ".deployment.identityFoundation.provisionState" "successful"
) ||
    (
        put-value ".deployment.identityFoundation.provisionState" "failed" &&
            echo "Deployment of Identity Provider failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

put-value ".deployment.iefPolicies.provisionState" "provisioning"
# Uploading IEF custom policies
(
    "${SCRIPT_DIR}/upload-ief-policies.sh" &&
        put-value ".deployment.iefPolicies.provisionState" "successful"
) ||
    (
        put-value ".deployment.iefPolicies.provisionState" "failed" &&
            echo "Upload of IEF policies failed." |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

# Adding OIDC Workflow for GitHub Actions
put-value ".deployment.oidc.provisionState" "provisioning"
(
    "${SCRIPT_DIR}/create-oidc-workflow-github-action.sh" &&
        put-value ".deployment.oidc.provisionState" "successful"
) ||
    (
        echo "OIDC Workflow for GitHub Actions failed." &&
            put-value ".deployment.oidc.provisionState" "failed" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    )

# end of script - which means running the clean-up script before exiting
exit 0

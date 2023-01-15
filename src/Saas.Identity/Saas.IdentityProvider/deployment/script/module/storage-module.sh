#!/usr/bin/env bash

# loading modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"

function storage-account-exist() {
    local resource_group_name="$1"
    local storage_account_name="$2"

    echo "Checking if storage account ${storage_account_name} exists..." \
        | log-output \
            --level info

    if [[ -z "${storage_account_name}" \
        || "${storage_account_name}" == null \
        || "${storage_account_name}" == "null" ]]; then

        false
        return
    fi

    response="$( az storage account show \
        --resource-group "${resource_group_name}" \
        --name "${storage_account_name}" 2> /dev/null \
        || false; return )"

    if [[ -n "${response}" ]]; then
        true
        return
    else
        false
        return
    fi    
}

function storage-create() {
    local resource_group_name="$1"
    local storage_account_name="$2"
    local location="$3"
    local storage_container_name="$4"
    local subscription_id="$5"
    local user_principal_id="$6"

    echo "Provisioning storage account ${storage_account_name}..." \
        | log-output \
            --level info \
            --header "Creating storage account"

    az storage account create \
        --resource-group "${resource_group_name}" \
        --name "${storage_account_name}" \
        --location "${location}" \
        --sku Standard_LRS \
        --kind StorageV2 \
        | log-output \
            --level info \
        || echo "Failed to create storage account ${storage_account_name}" \
            | log-output \
                --level error \
                --header "Critical error" \
            || exit 1

    echo "Storage account ${storage_account_name} created" \
        | log-output \
            --level info \

    echo "Assigning user 'contributor' access to storage account..." \
        | log-output \
            --level info
   
    az role assignment create \
        --role "Storage Blob Data Contributor" \
        --assignee "${user_principal_id}" \
        --scope "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Storage/storageAccounts/${storage_account_name}" \
        | log-output \
            --level info \
        || echo "Failed to assign user access to storage account" \
            | log-output \
                --level error \
                --header "Critical error" \
            || exit 1

    echo "Allowing Role Assignment 60 seconds to propagate..." \
        | log-output \
            --level info

    sleep 60

    echo "Provisioning storage container ${storage_container_name}..." \
        | log-output \
            --level info \

    az storage fs create \
        --name "${storage_container_name}" \
        --account-name "${storage_account_name}" \
        --auth-mode login \
        | log-output \
            --level info \
        || echo "Failed to create storage container ${storage_container_name}" \
            | log-output \
                --level error \
                --header "Critical error" \
            || exit 1

    echo "Creating 'log' directory in ${storage_container_name}..." \
        | log-output \
            --level info

    az storage fs directory create \
        --file-system "${storage_container_name}" \
        --account-name "${storage_account_name}" \
        --name "log" \
        --auth-mode login \
        | log-output \
            --level info \
            || echo "Failed to create directory in Azure Blob Storage." \
                | log-output \
                    --level error \
                    --header "Critical error"
}

function backup-to-azure-blob-storage() {
    local directory="$1"

    local storage_account_name
    local container_name

    storage_account_name=$(get-value ".deployment.storage.name")
    container_name=$(get-value ".deployment.storage.containerName")

    echo "Backing up logs in '${directory}' to Azure Blob Storage '${storage_account_name}/${container_name}'." \
        | log-output \
            --level info \
            --header "Backup to Azure Blob Storage"

    zip_file_name="${directory}/log-${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}.zip"

    zip -r -j "${zip_file_name}" "${directory}" > /dev/null \
        || echo "Failed to zip logs." \
            | log-output \
                --level error \
                --header "Critical error" \
            || exit 1

    az storage fs file upload \
        --account-name "${storage_account_name}" \
        --file-system "${container_name}" \
        --path "log/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}.zip" \
        --source "${zip_file_name}" \
        --auth-mode login \
        | log-output \
            --level info \
            || echo "Failed to upload logs to Azure Blob Storage." \
                | log-output \
                    --level error \
                    --header "Backup to Azure Blob Storage" \
                || exit 1
}


#!/bin/bash

# loading script modules into current shell
source "./script/config-module.sh"
source "./script/resource-module.sh"
source "./script/constants-module.sh"
source "./script/log-module.sh"

function certificate-exist() {
    local key_name="$1"

    key_vault_name="$( get-value ".deployment.keyVault.name" )"

    state="$( az keyvault certificate list \
        --vault-name "${key_vault_name}" \
        --include-pending \
        --query \
            "[?name=='${key_name}'] \
            .{Name:name} \
            | [0]" \
        --output tsv \
        || false ; return )"

    if [[ "${state}" = "${key_name}" ]]; then
        true
        return
    else   
        false
        return
    fi
}

function secret-exist() {
    local key_name="$1"
    local key_vault_name="$2"

    count="$( az keyvault secret list \
        --vault-name "${key_vault_name}" \
        --query \
            "[?name=='${key_name}']" \
            | jq '. | length' \
            || false ; return )"

    if [[ "${count}" -gt 0 ]]; then
        true
        return
    else   
        false
        return
    fi
}

function create-key-vault() {
    local key_vault_name="$1"
    local resource_group="$2"
    local key_vault_type_name="Microsoft.KeyVault/vaults"

    echo "Checking if the Key Vault have already been successfully created..." \
        | log-output \
            --level info

    if ! resource-exist "${key_vault_type_name}" "${key_vault_name}" ; then
        
        echo "No Key Vault found." | log-output --level info
        echo "Depolying Key Vault using bicep..." | log-output --level info

        az deployment group create \
            --resource-group "${resource_group}" \
            --template-file ./bicep/create-keyVault.bicep \
            --parameters \
                keyVaultName="${key_vault_name}" \
                | log-output\
            || echo "Failed to deploy Key Vault: $?" \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                    | exit 1
        echo "Key Vault Provisining successfully." | log-output --level success
    else
        echo "Existing Key Vault found." | log-output --level success
    fi
}

function init-key-vault-certificate-template() {
    local b2c_name="$1"

    # getting default certificate policy and saving it to json file
    az keyvault certificate get-default-policy \
        > "${CERTIFICATE_POLICY_FILE}" \
        || echo "Failed to get default certificate policy: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    # patching certificate policy
    put-certificate-value '.keyProperties.keySize' "4096"
    put-certificate-value '.x509CertificateProperties.subject' "CN=${b2c_name}"
}

function add-certificate-to-vault() {
    local key_name="$1"
    local key_vault_name="$2"
    local output_dir="$3"

    # check if certificate already exist and create it if not
    if ! certificate-exist "${key_name}" ; then
        echo "Creating a self-signing certificate for ${key_name}..." | log-output

        az keyvault certificate create \
            --name "${key_name}" \
            --vault-name "${key_vault_name}" \
            --policy "@${CERTIFICATE_POLICY_FILE}" 1> /dev/null \
            || echo "Failed to create self-signing certificate for ${key_name}." \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                    | exit 1
    else
        if [[ -f "${output_dir}/${key_name}" ]]; then
            sudo rm -f "${output_dir}/${key_name}"
        fi
    fi

    return
}

function get-certificate-public-key() {
    local key_name="$1"
    local key_vault_name="$2"
    local user="$3"
    local output_dir="$4"
    
    certificate_path="${output_dir}/${key_name}"

    download_dir="${HOME}/cert-temp"
    mkdir -p "${download_dir}"

    download_path="${download_dir}/${key_name}.crt"
    
    echo "Downloading self-signing public key certificate for ${key_name}..." \
        | log-output --level info

    az keyvault certificate download \
        --name "${key_name}" \
        --vault-name "${key_vault_name}" \
        --encoding "PEM" \
        --file "${download_path}" \
        > /dev/null ||
        echo "Failed to download self-signing public key certificate for ${key_name}: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    # change owner of the file to the user
    sudo mv "${download_path}" "${certificate_path}" \
        || echo "Failed to move certificate file to ${certificate_path}: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    sudo chown "${user}" "${certificate_path}" \
        || echo "Failed to change owner of the certificate file to ${user}: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    echo "${certificate_path}"
    return
}

function add-a-secret-to-vault() {
    local key_name="$1"
    local key_vault_name="$2"
    local user="$3"
    local output_path="$4"

    if secret-exist "${key_name}" "${key_vault_name}"; then

        echo "Downloading secret for ${key_name}..." | log-output --level info
        
        secret="$( az keyvault secret show \
            --vault-name "${key_vault_name}" \
            --name "${key_name}" \
            --query "value" \
            --output tsv \
            || echo "Failed to download secret for ${key_name}: $?" \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                    | exit 1 )"
    else
        secret="$( openssl rand -base64 26 )"

        echo "Uploading new secret for ${key_name}..." | log-output --level info

        az keyvault secret set \
            --name "${key_name}" \
            --vault-name "${key_vault_name}" \
            --value "${secret}" \
            > /dev/null \
            || echo "Failed to set new secret for ${key_name}: $?" \
                | log-output \
                    --level error \
                    --header "Critical Error" \
                    | exit 1
    fi

    secret_path="${output_path}/${key_name}.secret"
    
    # write secret to file in user home directory
    echo "${secret}" | sudo tee "${secret_path}" > /dev/null \
        || echo "Failed to write secret to file: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    # change owner of the file to the user
    sudo chown "${user}" "${secret_path}" \
        || echo "Failed to change owner of the secret file to ${user}: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

    echo "${secret_path}"
    return
}
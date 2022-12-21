#!/bin/bash
set -u -e -o pipefail

# loading script modules into current shell
source "./script/config-module.sh"
source "./script/resource-module.sh"
source "./script/key-vault-module.sh"
source "./script/log-module.sh"

resource_group="$( get-value ".deployment.resourceGroup.name" )"
key_vault_name="$( get-value ".deployment.keyVault.name" )"
key_name="$( get-value ".deployment.keyVault.key.name" )"
b2c_name="$( get-value ".deployment.azureb2c.name" )"

echo "Provisioning Key Vault..." | log-output --level info --header "Key Vault"
create-key-vault "${key_vault_name}" "${resource_group}"

echo "Getting or creating secrets..." | log-output --level info --header "Key Vault Certificates and Secrets"
# initialize key vault certificate template
init-key-vault-certificate-template "${b2c_name}"

# get app registrations
app_registrations="$( get-value ".appRegistrations" )"

# read each app_registration into an array
readarray -t app_reg_array < <( jq -c '.[]' <<< "${app_registrations}" )

# get B2C config user details
b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
b2c_config_usr_certificates_path="$( get-value ".deployment.azureb2c.certificateDir" )"

# loop through each app_registration and create a certificate if needed
for app_reg in "${app_reg_array[@]}"; do
    has_certificate="$( jq -r '.certificate'    <<< "${app_reg}" )"
    app_name="$( jq -r '.name'                  <<< "${app_reg}" )"

    if [[ "${has_certificate}" == "true" ]]; then

        add-certificate-to-vault \
            "${app_name}" \
            "${key_vault_name}" \
            "${b2c_config_usr_certificates_path}"

        # echo -e "Downloading PEM formatted public key for self-signing certificate for ${app_name}..." | log-output
        certificates_path="$( get-certificate-public-key \
            "${app_name}" \
            "${key_vault_name}" \
            "${b2c_config_usr_name}" \
            "${b2c_config_usr_certificates_path}" )"
        
        put-public-key-path "${app_name}" "${certificates_path}"
    fi
done

# get B2C policy keys
policy_keys="$( get-value ".azureb2c.policyKeys" )"

# read each policy key into an array
readarray -t policy_key_array < <( jq -c '.[]' <<< "${policy_keys}" )

# get service principal details
service_principal_name="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
service_principal_secrets_path="$( get-value ".deployment.azureb2c.servicePrincipal.secretDir" )"

# loop through each policy key and create a secret if needed
for policy_key in "${policy_key_array[@]}"; do
    options="$( jq -r '.options'        <<< "${policy_key}" )"
    has_secret="$( jq -r '.hasSecret'   <<< "${policy_key}" )"

    if [[ "${options}" == "Manual" && "${has_secret}" == "true" ]]; then
        key_name="$( jq -r '.name' <<< "${policy_key}" )"

        secret="$( add-a-secret-to-vault \
            "${key_name}" \
            "${key_vault_name}" \
            "${service_principal_name}" \
            "${service_principal_secrets_path}" )"

        put-policy-key-secret-path "${key_name}" "${secret}"
    fi
done

echo "Key Vault Certificats and Secrets Completed Successfully" | log-output --level success

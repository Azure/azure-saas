#!/usr/bin/env bash
set -u -e -o pipefail

# loading script modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/resource-module.sh"
source "$SCRIPT_MODULE_DIR/key-vault-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"

resource_group="$( get-value ".deployment.resourceGroup.name" )"
key_vault_name="$( get-value ".deployment.keyVault.name" )"
b2c_name="$( get-value ".deployment.azureb2c.domainName" )"

echo "Provisioning Key Vault..." | log-output --level info --header "Key Vault"
create-key-vault "${key_vault_name}" "${resource_group}"

echo "Getting and/or creating app certificates and secrets..." | log-output --level info --header "Key Vault Certificates and Secrets"
# initialize key vault certificate template
init-key-vault-certificate-template "${b2c_name}"

# get app registrations
app_registrations="$( get-value ".appRegistrations" )"

# read each app_registration into an array
readarray -t app_reg_array < <( jq -c '.[]' <<< "${app_registrations}" )

# get B2C config user details
b2c_config_usr_name="$( get-value ".deployment.azureb2c.username" )"
b2c_config_usr_certificates_path="$( get-user-value "${b2c_config_usr_name}" "${CERTIFICATE_DIR_NAME}" )"

# loop through each app_registration and create a certificate if needed
for app_reg in "${app_reg_array[@]}"; do
    has_certificate="$( jq -r '.certificate'    <<< "${app_reg}" )"
    app_name="$( jq -r '.name'                  <<< "${app_reg}" )"

    cert_name="cert-${app_name}"

    if [[ "${has_certificate}" == "true" ]]; then

        cert_name="$( create-certificate-in-vault \
            "${cert_name}" \
            "${key_vault_name}" \
            "${b2c_config_usr_certificates_path}" )"

        certificates_path="$( get-certificate-public-key \
            "${cert_name}" \
            "${key_vault_name}" \
            "${b2c_config_usr_certificates_path}" )"
        
        put-key-certificate-path "${app_name}" "${certificates_path}"

        put-certificate-key-name "${app_name}" "${cert_name}"
    fi
done

# get B2C policy keys
policy_keys="$( get-value ".azureb2c.policyKeys" )"

# read each policy key into an array
readarray -t policy_key_array < <( jq -c '.[]' <<< "${policy_keys}" )

# get service principal details
service_principal_name="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"
service_principal_secrets_path="$( get-user-value "${service_principal_name}" "${SECRET_DIR_NAME}" )"

# loop through each policy key and create a secret if needed
for policy_key in "${policy_key_array[@]}"; do
    options="$( jq -r '.options'        <<< "${policy_key}" )"
    has_secret="$( jq -r '.hasSecret'   <<< "${policy_key}" )"

    if [[ "${options}" == "Manual" && "${has_secret}" == "true" ]]; then
        policy_name="$( jq -r '.name' <<< "${policy_key}" )"

        secret_path="$( add-a-secret-to-vault \
            "${policy_name}" \
            "${key_vault_name}" \
            "${service_principal_secrets_path}" )"

        put-policy-key-secret-path "${policy_name}" "${secret_path}"
    fi
done

echo "Key Vault Certificats and Secrets Completed Successfully" \
    | log-output \
        --level success

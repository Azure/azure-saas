#!/usr/bin/env bash
set -u -e -o pipefail


# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/resource-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
}

resource_group="$( get-value ".deployment.resourceGroup.name" )"

b2c_location="$( get-value ".initConfig.azureb2c.location" )"
b2c_country_code="$( get-value ".initConfig.azureb2c.countryCode" )"
b2c_sku_name="$( get-value ".initConfig.azureb2c.skuName" )"
b2c_tier="$( get-value ".initConfig.azureb2c.tier" )"

b2c_display_name="$( get-value ".deployment.azureb2c.displayName" )"
b2c_name="$( get-value ".deployment.azureb2c.domainName" )"

b2c_type_name="Microsoft.AzureActiveDirectory/b2cDirectories"

if ! resource-exist "${b2c_type_name}" "${b2c_name}" ; then
    echo "No B2C directory found." \
        | log-output \
            --level info
    echo "Deploying Azure AD B2C Directory using bicep..." \
        | log-output \
            --level info

    az deployment group create \
        --resource-group "${resource_group}" \
        --template-file "${BICEP_DIR}/deployAzureB2c.bicep" \
        --output none \
        --parameters \
            location="${b2c_location}" \
            countryCode="${b2c_country_code}" \
            displayName="${b2c_display_name}" \
            name="${b2c_name}" \
            skuName="${b2c_sku_name}" \
            tier="${b2c_tier}" \
        || echo "Azure B2C deployment failed." | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

    echo "Provisionning of Azure B2C tenant Successful." | log-output --level success
    
else
    echo "Existing Azure B2C tenant found and will be used." | log-output --level success
fi
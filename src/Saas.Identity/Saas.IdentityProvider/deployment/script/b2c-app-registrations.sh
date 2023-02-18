#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/app-reg-module.sh"
    source "$SHARED_MODULE_DIR/colors-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/tenant-login-module.sh"
}

b2c_tenant_name="$(get-value ".deployment.azureb2c.domainName")"

# login to the B2C tenant
echo "Logging into B2C tenant ${b2c_tenant_name}." |
    log-output \
        --level info \
        --header "Azure B2C Tenant Login"

log-into-b2c "${b2c_tenant_name}" ||
    echo "Azure B2C tenant login failed." |
    log-output \
        --level error \
        --header "Critical error" ||
    exit 1

echo "Azure B2C tenant login successful." |
    log-output \
        --level success

echo "Adding app registrations to Azure B2C tenant." |
    log-output \
        --level info \
        --header "Azure B2C App Registrations"

# create the app registrations
declare -i scopes_length
declare -i permissions_length

# read each item in the JSON array to an item in the Bash array
readarray -t app_reg_array < <(jq --compact-output '.appRegistrations[]' "${CONFIG_FILE}")

# counter for iterations on Bash array - for testing purposes
# declare -i i
# i=1

b2c_tenant_name="$(get-value ".deployment.azureb2c.name")" ||
    echo "Azure B2C tenant namenot found." |
    log-output \
        --level error \
        --header "Critical error" ||
    exit 1

echo "Setting instance ${b2c_tenant_name}.b2clogin.com" |
    log-output \
        --level info \
        --header "Azure B2C Instance"

put-value ".deployment.azureb2c.instance" "https://${b2c_tenant_name}.b2clogin.com"

# iterate through the Bash array of app registrations
for app in "${app_reg_array[@]}"; do
    app_name=$(jq --raw-output '.name' <<<"${app}")
    app_id=$(jq --raw-output '.appId' <<<"${app}")
    has_cert=$(jq --raw-output '.certificate' <<<"${app}")
    has_secret=$(jq --raw-output '.hasSecret' <<<"${app}")
    redirect_uri=$(jq --raw-output '.redirectUri' <<<"${app}")
    redirect_type=$(jq --raw-output '.redirectType' <<<"${app}")
    logout_uri=$(jq --raw-output '.logoutUri' <<<"${app}")
    app_id_uri=$(jq --raw-output '.applicationIdUri' <<<"${app}")
    sign_in_audience=$(jq --raw-output '.signInAudience' <<<"${app}")
    id_tokens=$(jq --raw-output '.idTokens' <<<"${app}")
    is_allow_public_client_flows=$(jq --raw-output '.isAllowPublicClientFlows' <<<"${app}")
    permissions=$(jq --raw-output '.permissions' <<<"${app}")
    permissions_length=$(jq '.permissions | length' <<<"${app}")
    scopes=$(jq --raw-output '.scopes' <<<"${app}")
    scopes_length=$(jq --raw-output '.scopes | length' <<<"${app}")

    display_name="${app_name}"

    echo "Provisioning app registration for: ${display_name}..." |
        log-output \
            --level info \
            --header "${display_name}"

    if app-exist "${app_id}"; then
        echo "App registration for ${app_name} already exist. If you made changes or updated the certificate, you will have to delete the app registration to use this script to update it. " |
            log-output --level info
        continue
    fi

    if [[ -n "${redirect_uri}" &&
        ! "${redirect_uri}" == null &&
        ! "${redirect_uri}" == "null" ]]; then

        if [[ "${redirect_type}" == "web" ]]; then

            # create app with redirect uri
            echo "Creating app with web redirect_uri: ${redirect_uri}" | log-output --level info

            app_json="$(az ad app create \
                --display-name "${display_name}" \
                --web-redirect-uris "${redirect_uri}" \
                --only-show-errors \
                --query "{Id:id, AppId:appId}" ||
                echo "Failed to create app with redirect uri: ${redirect_uri}" |
                log-output \
                    --level error \
                    --header "Critical error" ||
                exit 1)"

        elif [[ "${redirect_type}" == "publicClient" ]]; then

            # create app with redirect uri
            echo "Creating app with public client redirect_uri: ${redirect_uri}" | log-output --level info

            app_json="$(az ad app create \
                --display-name "${display_name}" \
                --public-client-redirect-uris "${redirect_uri}" \
                --only-show-errors \
                --query "{Id:id, AppId:appId}" ||
                echo "Failed to create app with redirect uri: ${redirect_uri}" |
                log-output \
                    --level error \
                    --header "Critical error" ||
                exit 1)"
        fi
    else
        # create app registration without redirect uri
        echo "Creating app registration without redirect uri" | log-output --level info
        app_json="$(az ad app create \
            --display-name "${display_name}" \
            --only-show-errors \
            --query "{Id:id, AppId:appId}" ||
            echo "Failed to create app without redirect uri" |
            log-output \
                --header "Critical error" ||
            exit 1)"
    fi

    echo "App created: ${app_json}" |
        log-output \
            --level success

    obj_id=$(jq --raw-output '.Id' <<<"${app_json}")
    app_id=$(jq --raw-output '.AppId' <<<"${app_json}")

    # add appId to config
    put-app-id "${app_name}" "${app_id}"
    put-app-object-id "${app_name}" "${obj_id}"

    if [[ -n "${logout_uri}" &&
        ! "${logout_uri}" == null &&
        ! "${logout_uri}" == "null" ]]; then

        echo "Adding logout url: '${logout_uri}' for app with id '${app_id}'." |
            log-output \
                --level info

        add-signout-url "${obj_id}" "${logout_uri}"
    fi

    # adding sign-in audience when set
    if [[ "${sign_in_audience}" == "single" ]]; then
        az ad app update \
            --id "${app_id}" \
            --set signInAudience="AzureADMyOrg" \
            --only-show-errors |
            log-output \
                --level info ||
            echo "Failed to add single sign-in audience (single tenant) to app $app_name, ${app_id}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi

    if [[ "${sign_in_audience}" == "multiple" ]]; then
        az add app update \
            --id "${app_id}" \
            --set signInAudience="AzureADMultipleOrgs" \
            --only-show-errors |
            log-output \
                --level info ||
            echo "Failed to add multiple sign-in audience (multi tenant) to app $app_name, ${app_id}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi

    # add id-tokens to app registration if set
    if [[ "${id_tokens}" == true || "${id_tokens}" == "true" ]]; then
        az ad app update \
            --id "${app_id}" \
            --enable-id-token-issuance true \
            --only-show-errors |
            log-output \
                --level info ||
            echo "Failed to add id-tokens to app $app_name, ${app_id}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi

    ## add public client flows to app registration if set
    if [[ "${is_allow_public_client_flows}" == true || "${is_allow_public_client_flows}" == "true" ]]; then
        az ad app update \
            --id "${app_id}" \
            --is-fallback-public-client true \
            --only-show-errors |
            log-output \
                --level info ||
            echo "Failed to add public client flows to app $app_name, ${app_id}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi

    # add certificate to app registration if cert is true
    if [[ "${has_cert}" == true || "${has_cert}" == "true" ]]; then
        echo "Adding public key certificate for: ${app_name}..." |
            log-output --level info

        cert_path=$(jq --raw-output '.publicKeyPath' <<<"${app}")

        az ad app credential reset \
            --id "${obj_id}" \
            --cert "@${cert_path}" \
            --only-show-errors |
            log-output \
                --level info ||
            echo "Failed to add certificate to app $app_name, ${app_id}: $?" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1

        echo "Certificate added for: ${app_name}" |
            log-output \
                --level success
    fi

    # add secret to app registration if cert is true
    if [[ "${has_secret}" == true || "${has_secret}" == "true" ]]; then
        echo "Adding secret for: ${app_name}..." |
            log-output --level info

        secret_path=$USER/$ASDK_CURRENT_USER/.secret/$app_name.secret

        az ad app credential reset \
            --id "${obj_id}" \
            --display-name "${app_name}" \
            --end-date 9999-12-31 \
            --query password \
            --output tsv >"${secret_path}" ||
            echo "Failed to add secret to app $app_name, ${app_id}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1

        put-secret-path "${app_name}" "${secret_path}"

        echo "Secret added for: ${app_name}" |
            log-output \
                --level success
    fi

    # add identifier uri when scopes are present
    if [[ (($scopes_length -gt 0)) ]]; then
        echo "Adding identifier uri for: ${app_name}..." |
            log-output \
                --level info

        az ad app update --id "${app_id}" \
            --only-show-errors \
            --identifier-uris "${app_id_uri}" ||
            echo "Failed to update app $app_name, ${app_id}: $?" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1

        echo "Identifier added: ${app_id_uri}" |
            log-output \
                --level success

        echo "Adding ${scopes_length} permission scopes to for ${app_name}." |
            log-output \
                --level info

        add-permission-scopes "${obj_id}" "${app_name}" "${scopes}"

        echo "Permissions added for: ${app_name}" |
            log-output \
                --level success

        # to be able to consent to the new scopes a service principal needs to be created for the app registration
        echo "Creating service principal for: ${app_name}..." |
            log-output \
                --level info

        az ad sp create \
            --id "${app_id}" \
            --only-show-errors >/dev/null || echo "Failed to create service principal for app ${app_name}" |
            log-output \
                --level error \
                --header "Critical error" ||
            exit 1
    fi

    # add required resource permissions when permissions are present
    if [[ (($permissions_length -gt 0)) ]]; then
        echo "Adding required resource access for: ${app_name}..." |
            log-output \
                --level info

        add-required-resource-access "${permissions}" "${app_id}"

        echo "Required resource access added for: ${app_name}" |
            log-output \
                --level success
    fi

    # break loop on counter - for debugging purposes only
    # if [[ (( $i == 2 )) ]]; then
    #     exit 1
    # fi
    # ((++i))
done

#!/usr/bin/env bash


# load config-module into current shell
source "$SCRIPT_MODULE_DIR/config-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
source "$SCRIPT_MODULE_DIR/user-module.sh"

function is-logged-into-tenant-by-name() {
    local tenant_domain_name="$1" ;

    logged_into_domain_id="$( az rest \
        --method get \
        --url https://graph.microsoft.com/v1.0/domains \
        --query 'value[?isDefault].id' -o tsv 2> /dev/null \
          || false; return )"

    if [[ -n "${logged_into_domain_id}" ]] \
        && [[ "${tenant_domain_name}" == "${logged_into_domain_id}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function is-user-logged-in() {   
    if az account get-access-token > /dev/null 2>&1; then
        return
        true
    else
        return
        false
    fi
}

function is-logged-into-tenant-by-id() {
    local tenant_domain_name="$1"

    if is-user-logged-in; then

        logged_into_tenant_id="$( az account show \
        --query "{TenantId:tenantId}" \
        --output tsv 2> /dev/null \
            || false | return )"

        if [[ -n "${logged_into_tenant_id}" ]] \
            && [[ "${tenant_id}" == "${logged_into_tenant_id}" ]]; then
            true
            return
        fi
    else
        false
        return
    fi 
}

function log-in-error() {
    echo "Login failed. Please re-run deployment script to try again."  \
        | log-output \
            --level error \
            --header "Critical Error"
    exit 1
}

function log-into-main() { 
    local tenant_id="$1"
    local subscription_id="$2"

    if ! [[ "${tenant_id}" == "null" ]] \
        && [[ -n "${tenant_id}" ]] ; then

        if ! is-logged-into-tenant-by-id "${tenant_id}" ; then
            echo "You are not currently logged in to tenant: ${tenant_id}." \
                | log-output \
                    --level info \
                    --header "Login"

            echo "Please be patient, it sometimes take a little while for the login to start..." \
                | log-output \
                    --level warning

            echo "You must be logged into your Azure tenant to continue." \
                | log-output \
                    --level info
            
            sleep 1

            az login \
                --use-device-code \
                --tenant "${tenant_id}" \
                --output none \
                    || echo "Login failed. Please re-run deployment script to try again."  \
                        | log-output \
                            --level error \
                            --header "Critical Error" \
                            || exit 1

            tenant_id="$( az account show \
                --query "{tenantId:tenantId}" \
                --output tsv \
                    || log-in-error )"

            echo "Logged into tenant ID: ${tenant_id}" | log-output --level info
                
            if [[ -z "${tenant_id}" ]] ; then
                    log-in-error
            fi
        fi
    else
        echo "You have not specified a tenant id in the configuration file." \
            | log-output \
                --level error \
                --header "Critical Error"
        exit 1
    fi

    echo "Setting account subscription to ${subscription_id}." | log-output --level info

    az account \
        set --subscription "${subscription_id}" \
        | log-output --level info \
            || log-in-error

    echo "You are logged in to tenant: ${tenant_id}." \
            | log-output \
                --level success
}

function log-into-b2c() { 
    local tenant_name="$1"

    if ! [[ "${tenant_name}" == "null" ]] \
        && [[ -n "${tenant_name}" ]] ; then

        if is-logged-into-tenant-by-name "${tenant_name}" ; then
            echo "You are already logged in to the Azure B2C tenant: ${tenant_name}" \
                | log-output \
                    --level info
            return 
        fi
    fi

    echo "You are not currently logged in to the Azure B2C tenant: ${tenant_name}." \
        | log-output \
            --level info

    echo "Please be patient, it sometimes take a little while for the login prompt to appear..." \
        | log-output \
            --level warning

    echo "You must be logged into your Azure B2C tenant to continue." \
        | log-output \
            --level info

    sleep 1
    
    az login \
        --use-device-code \
        --tenant "${tenant_name}" \
        --only-show-errors \
        --allow-no-subscription \
        --output none \
            || log-in-error

    tenant_id="$( az account show \
        --query "{tenantId:tenantId}" \
        --output tsv \
            || log-in-error )"

    if [[ -z "${tenant_id}" ]] ; then
        log-in-error
    fi

    cache-session

    put-value ".deployment.azureb2c.tenantId" "${tenant_id}"       
}

function cache-session() {
    set +u
    if [[ ! $ASDK_CACHE_AZ_CLI_SESSIONS == true ]] || [[ -z $ASDK_CURRENT_USER ]]; then
        set -u 
        echo "Caching setting not enabled or no current user set." \
            | log-output \
                --level warning \
                --header "Caching az cli session"
    else
        set -u 
        echo "Caching enabled: '$ASDK_CACHE_AZ_CLI_SESSIONS' for current user: '$ASDK_CURRENT_USER'" \
            | log-output \
                --level info \
                --header "Caching az cli session"
        set-cache-session "${ASDK_CURRENT_USER}"
    fi
}
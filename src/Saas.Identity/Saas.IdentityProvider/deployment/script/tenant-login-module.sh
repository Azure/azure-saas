#!/bin/bash


# load config-module into current shell
source "./script/config-module.sh"
source "./script/log-module.sh"

function is-logged-into-tenant_by_name() {
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

function is-logged-into-tenant_by_id() {
    local tenant_domain_name="$1"

    logged_into_tenant_id="$( az account show \
        --query "{TenantId:tenantId}" \
        --output tsv 2> /dev/null \
            || false; return )"

    if [[ -n "${logged_into_tenant_id}" ]] \
        && [[ "${tenant_id}" == "${logged_into_tenant_id}" ]]; then
        true
        return
    fi     

    false
    return   
}

function log-in-error() {
    echo "Login failed. Please re-run deployment script to try again."  \
        | log-output \
            --level error \
            --header "Critical Error"
    exit 1
}

function log-into-main() { 
    local subscription_id="$1" ;
    local tenant_id="$2";

    if ! [[ "${tenant_id}" == "null" ]] \
        && [[ -n "${tenant_id}" ]] ; then

        if ! is-logged-into-tenant_by_id "${tenant_id}" ; then
            echo "You are not current logged in to tenant: ${tenant_id}." \
                | log-output \
                    --level info \
                    --header "Login"

            echo "Please be patient, it sometimes take a little while for the login to start..." \
                | log-output \
                    --level warning

            echo "You must be logged into your Azure tenant to continue." \
                | log-output \
                    --level info
            
            sleep 1s

            az login \
                --tenant "${tenant_id}" \
                --output none \
                    || echo "Login failed. Please re-run deployment script to try again."  \
                        | log-output \
                            --level error \
                            --header "Critical Error" \
                            | exit 1

            tenant_id="$( az account show \
                --query "{tenantId:tenantId}" \
                --output tsv \
                    || log-in-error )"

            echo "Logged into tenant ID: ${tenant_id}" | log-output --level info
                
            if [[ -z "${tenant_id}" ]] ; then
                    log-in-error
            fi

            echo "Setting account subscription to ${subscription_id}." | log-output --level info

            az account \
                set --subscription "${subscription_id}" \
                | log-output --level info \
                    || log-in-error
        fi
    else
        echo "You have not specified a tenant id in the configuration file." \
            | log-output \
                --level error \
                --header "Critical Error"
        exit 1
    fi

    echo "You are logged in to tenant: ${tenant_id}." \
            | log-output \
                --level success
}

function log-into-b2c() { 
    local tenant_name="$1"

    if ! [[ "${tenant_name}" == "null" ]] \
        && [[ -n "${tenant_name}" ]] ; then

        if is-logged-into-tenant_by_name "${tenant_name}" ; then
            echo "You are already logged in to the Azure B2C tenant: ${tenant_name}" \
                | log-output \
                    --level info \
            return
        else
            echo "You are not current logged in to B2C tenant: ${tenant_name}." \
                | log-output \
                    --level info       
        fi
    fi

    echo "Please be patient, it sometimes take a little while for the login prompt to appear..." \
        | log-output \
            --level warning

    echo "You must be logged into your Azure B2C tenant to continue." \
        | log-output \
            --level info

    sleep 1s
    
    az login \
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

    put-value ".deployment.azureb2c.tenantId" "${tenant_id}"
}
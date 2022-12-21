#!/bin/bash

# load config-module into current shell
source "./script/config-module.sh"
source "./script/log-module.sh"

function resource-exist {
    local type_name="$1" ;
    local resource_name="$2" ;

    resource_group="$( get-value ".deployment.resourceGroup.name" )"

    state="$( az resource list \
        --resource-group "${resource_group}" \
        --query \
            "[?type=='${type_name}']\
            .{ProvisioningState:provisioningState, Name:name} \
                | [?Name=='${resource_name}'].ProvisioningState \
                | [0]" \
        --output tsv \
        || echo "Failed to get resource state"  \
            | log-output \
                --level warn ; false ; return )"

    if [[ ${state} = "Succeeded" ]]; then
        true
        return
    else   
        false
        return
    fi
}

function create-resource-group() {
    resource_group=$1
    location=$2
    
    # Creating Resource Group if it doesn't already exist
    ( az group create \
        --location "${location}" \
        --name "${resource_group}" > /dev/null \
        && echo "Resource group ${resource_group} provision complete." \
            | log-output \
                --level success ) \
        || echo "Failed to create resource group ${resource_group}."  \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1
}


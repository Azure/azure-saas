#!/usr/bin/env bash

# shellcheck disable=SC1091
source "$SHARED_MODULE_DIR/config-module.sh"

function federation-exist() {
    local app_id="$1"
    local subject="$2"

    response_subject="$( az ad app federated-credential list \
        --id "${app_id}" \
        --query "[?issuer == 'https://token.actions.githubusercontent.com'] \
            | [?contains(subject, '${subject}')].subject
            | [0]" \
        --output tsv \
        || false; return )"

    if [[ -n "${response_subject}" ]]; then
        true
        return
    else
        false
        return
    fi
}

function role-assignment-exist() {
    local resource_group_name="$1"
    local assign_object_id="$2"

    if [[ -z "${assign_object_id}" \
        || "${assign_object_id}" == null \
        || "${assign_object_id}" == "null" ]]; then

        false
        return
    fi

    federation_exist="$( az role assignment list \
        --resource-group "${resource_group_name}" \
        --query "[?contains(principalId, '${assign_object_id}')].principalId \
            | [0]=='${assign_object_id}'" 2> /dev/null \
        || false; return )"

    if [ "${federation_exist}" == "true" ]; then
        true
        return
    else
        false
        return
    fi
}

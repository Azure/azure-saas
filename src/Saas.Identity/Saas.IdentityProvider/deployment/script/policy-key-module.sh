#!/bin/bash

# include script modules into current shell
source "./script/config-module.sh"
source "./script/log-module.sh"

function create-policy-key-body() {
    local name="$1"
    local key_type="$2"
    local key_use="$3"
    local options="$4"
    local secret="$5"

    case "${key_use}" in
        "Signature")
            key_use="sig"
            ;;
        "Encryption")
            key_use="enc"
            ;;
        *)
            echo "Invalid option: ${key_use}. Must be Signature or Encryption"
            exit 1
            ;;
    esac

    case "${options}" in
        "Generate")
            case "${key_type}" in
                "RSA")
                    key_type="rsa"
                    ;;
                "OCT")
                    key_type="oct"
                    ;;
                *)
                    echo "Invalid option: ${key_type}. Must be RSA or OCT"
                    exit 1
                    ;;
            esac
            body_json="$( cat <<-END
{
    "option": "Generate",
    "id": "$name",
    "kty": "$key_type",
    "use": "$key_use"
}
END
) "

            ;;
        "Manual")
            body_json="$( cat <<-END
{   
    "option": "Manual",
    "id": "$name",
    "k": "$secret",
    "use": "$key_use"
}
END
) "
            ;;
        *)
            echo "Invalid option: ${options}. Must be Generate or Manual"
            exit 1
            ;;
            
    esac

    echo "${body_json}"
    return
}

function post-rest-request() {
    local uri="$1"
    local body="$2"

    az rest --method POST \
        --uri "${uri}" \
        --headers "Content-Type=application/json" \
        --body "${body}" 1> /dev/null\
        || echo "Failed to send request to $uri: $?" \
            | log-output \
                --level error \
                --header "Critical Error" \
                | exit 1

}

function create-policy-key-set() {
    local policy_key_body="$1"
    
    create_uri="https://graph.microsoft.com/beta/trustFramework/keySets"

    create_policy_key_body="$( jq -c ". \
        | to_entries \
        | map(select(.key == \"id\")) \
        | from_entries" \
        <<< "${policy_key_body}" )"

    echo "Create policy key-set body: ${create_policy_key_body}" | log-output --level info

    az rest --method POST \
        --uri "${create_uri}" \
        --headers "Content-Type=application/json" \
        --body "${create_policy_key_body}" 1> /dev/null \
        || echo "Failed to create policy key-set: $?" \
            | log-output \
                --level error \
                --header "Critical Error"
}

function generate-policy-key() {
        local id="$1"
        local policy_key_body="$2"

        generate_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${id}/generateKey"

        generate_body="$( jq 'del(.option) | del(.id)' <<< "${policy_key_body}" )"

        post-rest-request "${generate_uri}" "${generate_body}"
}

function upload-policy-secret() {
        local id="$1"
        local policy_key_body="$2"

        secret_update_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${id}/uploadSecret"

        secret_update_body="$( jq 'del(.option) | del(.id)' <<< "${policy_key_body}" )"

        post-rest-request "${secret_update_uri}" "${secret_update_body}"

}

function policy-key-exist() {
    local id="$1"

    get_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${id}"

    policy_key="$( az rest --method GET \
        --uri "${get_uri}" \
        --headers "Content-Type=application/json" \
        --output tsv 2> /dev/null \
        || exit 1 )" || return 1 # return false

    if [ -n "${policy_key}" ]; then
        true
        return
    else
        false
        return
    fi
}
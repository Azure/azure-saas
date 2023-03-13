#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
source "$SHARED_MODULE_DIR/config-module.sh"
source "$SHARED_MODULE_DIR/log-module.sh"

function create-policy-key-create-request() {
    local id="$1"

    body_json="$(
        cat <<-END
{
    "id": "B2C_1A_$id"
}
END
    ) "

    echo "${body_json}"
    return
}

function create-policy-key-body() {
    local name="$1"
    local key_type="$2"
    local key_use="$3"
    local options="$4"
    local secret="$5"

    nbf="$(date +%s)"
    exp="$(date -d "+2 year" +%s)"

    case "${key_use}" in
    "Signature")
        key_use_name="sig"
        ;;
    "Encryption")
        key_use_name="enc"
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
            key_type_name="rsa"
            ;;
        "OCT")
            key_type_name="oct"
            ;;
        *)
            echo "Invalid option: ${key_type}. Must be RSA or OCT"
            exit 1
            ;;
        esac
        body_json="$(
            cat <<-END
{
    "kty": "$key_type_name",
    "use": "$key_use_name"
}
END
        ) "

        ;;
    "Manual")
        body_json="$(
            cat <<-END
{   
    "k": "$secret",
    "use": "$key_use_name"
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

function create-policy-key-set() {
    local name="$1"
    local key_type="$2"
    local key_use="$3"
    local options="$4"
    local secret="$5"

    create_uri="https://graph.microsoft.com/beta/trustFramework/keySets"

    policy_key_create_body="$(create-policy-key-create-request "${name}")"

    echo "Creating policy key-set '${name}' with body: ${policy_key_create_body}" |
        log-output \
            --level info

    post-rest-request "${create_uri}" "${policy_key_create_body}" "POST"

    echo "Waiting 10 seconds for key-set to settle..." | echo-color --level info
    sleep 10

    local generate_uri

    if [[ "${options}" == "Generate" ]]; then
        generate_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${name}/generateKey"
    elif [[ "${options}" == "Manual" ]]; then
        generate_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${name}/uploadSecret"
    fi

    echo "Generating policy key for '${name}' with option '${options}'." |
        log-output \
            --level info

    policy_key_generate_body="$(create-policy-key-body "${name}" "${key_type}" "${key_use}" "${options}" "${secret}")"

    post-rest-request "${generate_uri}" "${policy_key_generate_body}" "POST"
}

function post-rest-request() {
    local uri="$1"
    local body="$2"
    local method="$3"

    az rest \
        --method "${method}" \
        --uri "${uri}" \
        --headers "Content-type=application/json" \
        --body "${body}" \
        --only-show-errors 1>/dev/null ||
        echo "Failed to send request to $uri" |
        log-output \
            --level error \
            --header "Critical Error" ||
        exit 1

}

function upload-custom-policy() {
    local id="$1"
    local policy_xml_path="$2"

    uri="https://graph.microsoft.com/beta/trustFramework/policies/${id}/\$value"
    headers="Content-Type=application/xml"

    # removing the BOM from the file or az rest will choke on it.
    # https://en.wikipedia.org/wiki/Byte_order_mark
    # sed -i '1s/^\xEF\xBB\xBF//' "${policy_xml_file}"

    target_dir="${HOME}/temp"
    mkdir -p "${target_dir}"
    target_file="${target_dir}/${policy_xml_path##*/}"

    dos2unix \
        --quiet \
        --remove-bom \
        --newfile "${policy_xml_path}" "${target_file}"

    body="$(cat "${target_file}")"

    az rest \
        --method PUT \
        --uri "${uri}" \
        --headers "${headers}" \
        --body "${body}" \
        --only-show-errors 1>/dev/null ||
        echo "Failed to upload policy file: ${policy_xml_path} " |
        log-output \
            --level error \
            --header "Critical Error"
}

function policy-key-exist() {
    local id="$1"

    get_uri="https://graph.microsoft.com/beta/trustFramework/keySets/B2C_1A_${id}"

    policy_key="$(az rest --method GET \
        --uri "${get_uri}" \
        --headers "Content-Type=application/json" \
        --output tsv 2>/dev/null ||
        exit 1)" || return 1 # return false

    if [ -n "${policy_key}" ]; then
        true
        return
    else
        false
        return
    fi
}

#!/usr/bin/env bash

# include script modules into current shell
source "constants.sh"
source "$SCRIPT_MODULE_DIR/util-module.sh"
source "$SCRIPT_MODULE_DIR/log-module.sh"
    
# load the appropriate user module for the OS
what_os="$( get-os )"

if [[ "${what_os}" == "linux" ]]; then
    source "${SCRIPT_MODULE_DIR}/linux-ubuntu/linux-user-module.sh"
elif [[ "${what_os}" == "macos" ]]; then
    source "${SCRIPT_MODULE_DIR}/macos/macos-user-module.sh"
else
    echo "Unsupported OS: ${what_os}" \
        | log-output \
            --level error \
            --header "Critical Error"
    exit 1
fi

function delete-user-context-data() {
    local user_name="$1"

    local context_dir
    context_dir="$( get-user-context-dir "${user_name}" )"

    # check if the context directory is valid to protect against deleting the wrong directory
    if is-valid-context-dir "${context_dir}/data" "${user_name}"; then
        sudo rm -r "${context_dir}/data"
    fi
}

function create-user-context() {
    local user_name="$1"
    local sub_path_name="$2"
    local sub_path="$3"
    local user_session_cachable="$4"

    local user_context_dir="${HOME}/${user_name}"
    local azure_context_dir="${user_context_dir}/.azure"

    delete-user-context-data "${user_name}"

    put-user-context "${user_name}" "${user_context_dir}"

    if [[ -d "${azure_context_dir}" ]]; then
        echo "Azure context directory already exist and will be used: ${azure_context_dir}" \
            | log-output \
                --level warning
    else
        mkdir -p "${azure_context_dir}" \
            || echo "Failed to create az cli context directory ${azure_context_dir}" \
                | log-output \
                    --level warning
    fi

    # when not running in container, set ownership of the context directory
    if ! [ -f /.dockerenv ]; then
        sudo chown -R "${USER}" "${azure_context_dir}"
    fi

    if [[ $ASDK_CACHE_AZ_CLI_SESSIONS == true ]]  && $user_session_cachable; then
        get-cached-session "${user_name}" "${azure_context_dir}"
    fi

    if [[ -n "${sub_path}" ]]; then
        mkdir -p "${user_context_dir}/data/${sub_path}" \
            || echo "Failed to create user sub directory ${user_context_dir}/data/${sub_path}" \
                | log-output --level warning

        put-user-value "${user_name}" "${sub_path_name}" "${user_context_dir}/data/${sub_path}"
    fi
}

# set the azure cli context directory for the specified user
function set-user-context() {
    local user_name="$1"
    local user_context_dir

    export ASDK_CURRENT_USER="${user_name}"

    user_context_dir="$( get-user-context-dir "${user_name}" )"
    local azure_context_dir="${user_context_dir}/.azure"

    set-azure-cli-context "${azure_context_dir}"
}

# set the azure cli context directory for default user
function reset-user-context() {
    restore-azure-cli-context
}

function set-cache-session() {
    local user_name="$1"

    # when running in container, current session
    if [ -f /.dockerenv ]; then
        mkdir -p /asdk/.cache/"${user_name}"
        cp -rf "${AZURE_CONFIG_DIR}"/msal_token_cache.* /asdk/.cache/"${user_name}"/
        cp -rf "${AZURE_CONFIG_DIR}"/azureProfile.json /asdk/.cache/"${user_name}"/
    # when running on the host, cache current session
    else
        sudo mkdir -p "$HOME"/asdk/.cache/"$user_name"
        sudo cp -rf "${AZURE_CONFIG_DIR}"/msal_token_cache.* "$HOME"/asdk/.cache/"$user_name"/
        sudo cp -rf "${AZURE_CONFIG_DIR}"/azureProfile.json "$HOME"/asdk/.cache/"$user_name"/
        sudo chown -R "${USER}" "$HOME"/asdk/.cache/"$user_name"
    fi
}

function get-cached-session() {
    local user_name="$1"
    local azure_context_dir="$2"

    set +u
    if [[ ! $ASDK_CACHE_AZ_CLI_SESSIONS == true ]]; then
        return
    fi
    set -u
    
    # when running in container, get previously cached sessions
    if [ -f /.dockerenv ]; then
        if [ -d "/asdk/.cache/$user_name" ]; then
            cp -f /asdk/.cache/"$user_name"/msal_token_cache.* "${azure_context_dir}"
            cp -f /asdk/.cache/"$user_name"/azureProfile.json "${azure_context_dir}"
        fi
    # when running on the host, get previously cached sessions
    else
        if [ -d "$HOME/asdk/.cache/$user_name" ]; then
            sudo cp -f "$HOME"/asdk/.cache/"$user_name"/msal_token_cache.* "${azure_context_dir}"
            sudo cp -f "$HOME"/asdk/.cache/"$user_name"/azureProfile.json "${azure_context_dir}"
        fi
    fi
}
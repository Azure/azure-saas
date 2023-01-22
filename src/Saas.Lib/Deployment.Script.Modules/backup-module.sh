#!/usr/bin/env bash

# shellcheck disable=SC1091
# include script modules into current shell
source "$SHARED_MODULE_DIR/log-module.sh"
source "$SHARED_MODULE_DIR/storage-module.sh"

function backup-config-beginning() {
    set +u
    if [[ -z "${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}" ]]; then
        now=$(date '+%Y-%m-%d--%H-%M-%S')
        export ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME="${now}"
    fi
    set -u

    backup_file="${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}/config.begin.json"

    echo "Backing up existing configuration file to: ${backup_file}" \
        | log-output \
            --level info \
            --header "Backup Configuration"

    mkdir -p "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}"
    cp "${CONFIG_FILE}" "${backup_file}"
}

function backup-config-end() {
    set +u
    if [[ -z "${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}" ]]; then
        now=$(date '+%Y-%m-%d--%H-%M-%S')
        export ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME="${now}"
    fi
    set -u

    cp "${CONFIG_FILE}" \
        "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}/config.end.json"

    cp "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" \
        "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}/"

    cp "${CERTIFICATE_POLICY_FILE}" \
        "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}/"
}

function backup-log() {
    local script_name="$1"
    
    set +u
    if [[ -z "${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}" ]]; then
        now=$(date '+%Y-%m-%d--%H-%M-%S')
        export ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME="${now}"
    fi
    set -u

    mv "${LOG_FILE_DIR}/deploy-${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}.log" \
        "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}/deploy.log"

    backup-to-azure-blob-storage \
        "${LOG_FILE_DIR}/${ASDK_ID_PROVIDER_DEPLOYMENT_RUN_TIME}" \
        "${script_name}"
}

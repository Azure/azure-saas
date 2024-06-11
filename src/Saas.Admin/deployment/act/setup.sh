#!/usr/bin/env bash

skip_docker_build=false
force_update=false

while getopts 'sf' flag; do
    case "${flag}" in
    s) skip_docker_build=true ;;
    f) force_update=true ;;
    *) skip_docker_build=false ;;
    esac
done

# shellcheck disable=SC1091
source "../constants.sh"

echo "Setting up the SaaS Admin Service API Act deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo mkdir -p "${ACT_SECRETS_DIR}"

sudo touch ${ACT_SECRETS_FILE}
sudo chown "${USER}" ${ACT_SECRETS_FILE}
sudo touch ${ACT_SECRETS_FILE_RG}
sudo chown "${USER}" ${ACT_SECRETS_FILE_RG}

if [ "${skip_docker_build}" = false ]; then
    echo "Building the deployment container."

    if [[ "${force_update}" == false ]]; then
        "${ACT_CONTAINER_DIR}"/build.sh -n "${ACT_CONTAINER_NAME}"
    else
        "${ACT_CONTAINER_DIR}"/build.sh -n "${ACT_CONTAINER_NAME}" -f
    fi
fi

echo "SaaS SaaS Admin Service API Act environment setup complete. You can now run the local deployment script using the command './deploy.sh'."

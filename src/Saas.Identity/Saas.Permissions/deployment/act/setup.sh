#!/usr/bin/env bash

skip_docker_build=false

while getopts s flag
do
    case "${flag}" in
        s) skip_docker_build=true;;
        *) skip_docker_build=false;;
    esac
done

# shellcheck disable=SC1091
source "../constants.sh"

echo "Setting up the SaaS Permissions Serivce API Act deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo mkdir -p "${ACT_SECRETS_DIR}"

sudo chmod +x ${ACT_DIR}/*.sh
sudo chmod +x ${SCRIPT_DIR}/*.sh
sudo chmod +x ${SCRIPT_DIR}/*.py
sudo touch ${ACT_SECRETS_FILE}
sudo chown "${USER}" ${ACT_SECRETS_FILE}

if [ "${skip_docker_build}" = false ]; then
    echo "Building the deployment container."
    ./build.sh ||
        {
            echo "Failed to build the deployment container. Please ensure that Docker is installed and running."
            exit 1
        }
fi

echo "SaaS Permissions Service API Act environment setup complete. You can now run the local deployment script using the command './deploy.sh'."
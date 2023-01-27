#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./../constants.sh"

echo "Setting up the SaaS Permissions Serivce API Act deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo chmod +x "${ACT_DIR}/*.sh"
sudo chmod +x "${SCRIPT_DIR}/*.sh"
sudo chmod +x "${SCRIPT_DIR}/*.py"
sudo touch "${SECRETS_FILE}"
sudo chown "${USER}" "${SECRETS_FILE}"

echo "Building Act container."
./build.sh ||
    {
        echo "Failed to build the deployment container. Please ensure that Docker is installed and running."
        exit 1
    }

echo
echo "SaaS Permissions Service API Act environment setup complete. You can now run the local deployment script using the command './run.sh'."
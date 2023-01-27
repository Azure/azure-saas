#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./constants.sh"

echo "Setting up the deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo chmod +x "${BASE_DIR}/*.sh"
sudo chmod +x "${SCRIPT_DIR}/*.sh"
sudo chmod +x "${SCRIPT_DIR}/*.py"

echo
echo "SaaS Permissions Service API environment setup complete. You can now run the deployment script using the command './run.sh'."
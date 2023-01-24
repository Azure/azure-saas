#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./constants.sh"

echo "Setting up the deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo chmod +x ./*.sh
sudo chmod +x ./script/*.sh
sudo chmod +x ./script/*.py

repo_base="$( git rev-parse --show-toplevel )"
docker_file_folder="${repo_base}/src/Saas.lib/Deployment.Container"

# redirect to build.sh in the Deployment.Container folder
sudo chmod +x "${docker_file_folder}/build.sh"

echo "Building the deployment container."
./build.sh ||
    {
        echo "Failed to build the deployment container. Please ensure that Docker is installed and running."
        exit 1
    }

echo "Setting up log folder..."
mkdir -p "$LOG_FILE_DIR"
sudo chown "${USER}" "$LOG_FILE_DIR"

echo "Setting up config folder..."
mkdir -p "${CONFIG_DIR}"
sudo chown "${USER}" "${CONFIG_DIR}"
sudo chown "${USER}" "${CONFIG_FILE}"
sudo chown "${USER}" "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}"

echo "Setting up policy folder..."
mkdir -p "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR}"
sudo chown "${USER}" "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR}"
sudo chown "${USER}" "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE}"

echo
echo "Setup complete. You can now run the deployment script using the command './run.sh'."
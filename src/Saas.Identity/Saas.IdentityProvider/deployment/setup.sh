#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./constants.sh"

echo "Setting up the deployment environment."
echo "Settings execute permissions on necessary scripts files."

(
    sudo chmod +x ./*.sh || exit 1
    sudo chmod +x ./script/*.sh || exit 1
    sudo chmod +x ./script/*.py || exit 1
) ||
    {
        echo "Failed to set execute permissions on the necessary scripts."
        exit 1
    }

repo_base="$(git rev-parse --show-toplevel)" ||
    {
        echo "Failed to get the root of the repository."
        exit 1
    }

docker_file_folder="${repo_base}/src/Saas.Lib/Deployment.Container"

# redirect to build.sh in the Deployment.Container folder
sudo chmod +x "${docker_file_folder}/build.sh" ||
    {
        echo "Failed to set execute permissions on the 'build.sh'  script."
        exit 1
    }

echo "Building the deployment container."
./build.sh ||
    {
        echo "Failed to build the deployment container. Please ensure that Docker is installed and running."
        exit 1
    }

(
    echo "Setting up log folder..."
    mkdir -p "$LOG_FILE_DIR" || exit 1
    sudo chown "${USER}" "$LOG_FILE_DIR" || exit 1

    echo "Setting up config folder..."
    mkdir -p "${CONFIG_DIR}" || exit 1
    sudo chown "${USER}" "${CONFIG_DIR}" || exit 1
    touch "${CONFIG_FILE}" || exit 1
    sudo chown "${USER}" "${CONFIG_FILE}" || exit 1
    touch "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" || exit 1
    sudo chown "${USER}" "${IDENTITY_FOUNDATION_BICEP_PARAMETERS_FILE}" || exit 1

    echo "Setting up policy folder..."
    mkdir -p "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR}" || exit 1
    sudo chown "${USER}" "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_ENVIRONMENT_DIR}" || exit 1
    touch "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE}" || exit 1
    sudo chown "${USER}" "${IDENTITY_EXPERIENCE_FRAMEWORK_POLICY_APP_SETTINGS_FILE}" || exit 1
) ||
    {
        echo "Failed to setting up folders with permissions."
        exit 1
    }

echo
echo "Setup complete. You can now run the deployment script using the command './run.sh'."

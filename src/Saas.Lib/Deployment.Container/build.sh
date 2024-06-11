#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./constants.sh"
source "$SHARED_MODULE_DIR/util-module.sh"

force_update=false

while getopts f flag; do
    case "${flag}" in
    f) force_update=true ;;
    *) force_update=false ;;
    esac
done

repo_base="$(git rev-parse --show-toplevel)"
docker_file_folder="${repo_base}/src/Saas.Lib/Deployment.Container"

architecture="$(uname -a)"

what_os="$(get-os)" ||
    echo "Unsupported OS: ${what_os}. This script support linux (ubxl.WSL 2.0 on Windows) and MacOS." |
    log-output \
        --level error \
        --header "Critical Error" ||
    exit 1

if [[ "${force_update}" == false ]]; then
    if [[ "${architecture}" == *"ARM64"* ]]; then
        if [[ "${what_os}" == "linux" ]]; then
            echo "Building for Linux on Arm)..."
            docker build --file "${docker_file_folder}/Dockerfile.ARM" --tag asdk-script-deployment:latest .
        elif [[ "${what_os}" == "macos" ]]; then
            echo "Building for MacOS on Apple Silicon"
            docker build --file "${docker_file_folder}/Dockerfile.AppleSilicon" --tag asdk-script-deployment:latest .
        else
            exit 1
        fi
    else
        echo "Building for Linux (incl. WSL 2.0 on Windows) on x86_64)..."
        docker build --file "${docker_file_folder}/Dockerfile" --tag asdk-script-deployment:latest .
    fi
else
    if [[ "${architecture}" == *"ARM64"* ]]; then
        if [[ "${what_os}" == "linux" ]]; then
            echo "Force building for Linux on Arm)..."
            docker build --no-cache --file "${docker_file_folder}/Dockerfile.ARM" --tag asdk-script-deployment:latest .
        elif [[ "${what_os}" == "macos" ]]; then
            echo "Force building for MacOS on Apple Silicon"
            docker build --no-cache --file "${docker_file_folder}/Dockerfile.AppleSilicon" --tag asdk-script-deployment:latest .
        else
            exit 1
        fi
    else
        echo "Force building for Linux (incl. WSL 2.0 on Windows) on x86_64)..."
        docker build --no-cache --file "${docker_file_folder}/Dockerfile" --tag asdk-script-deployment:latest .
    fi
fi

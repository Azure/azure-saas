#!/usr/bin/env bash

repo_base="$( git rev-parse --show-toplevel )"
docker_file_folder="${repo_base}/src/Saas.lib/Deployment.Container"

architecture="$( uname -a )"

if [[ "${architecture}" == *"ARM64"* ]]; then
    echo "Building for ARM64 (including Apple Sillicon)..."
    docker build --file "${docker_file_folder}/Dockerfile.ARM" --tag asdk-script-deployment:latest .
else
    docker build --file "${docker_file_folder}/Dockerfile" --tag asdk-script-deployment:latest .
fi

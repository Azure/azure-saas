#!/usr/bin/env bash

force_update=false

while getopts f flag
do
    case "${flag}" in
        f) force_update=true;;
        *) force_update=false;;
    esac
done

repo_base="$( git rev-parse --show-toplevel )"
docker_file_folder="${repo_base}/src/Saas.Lib/Deployment.Container"

architecture="$( uname -a )"

if [[ "${force_update}" == false  ]]; then
    if [[ "${architecture}" == *"ARM64"* ]]; then
        echo "Building for ARM64 (including Apple Sillicon)..."
        docker build --file "${docker_file_folder}/Dockerfile.ARM" --tag asdk-script-deployment:latest .
    else
        docker build --file "${docker_file_folder}/Dockerfile" --tag asdk-script-deployment:latest .
    fi
else
    if [[ "${architecture}" == *"ARM64"* ]]; then
        echo "Building for ARM64 (including Apple Sillicon)..."
        docker build --no-cache --file "${docker_file_folder}/Dockerfile.ARM" --tag asdk-script-deployment:latest .
    else
        docker build --no-cache --file "${docker_file_folder}/Dockerfile" --tag asdk-script-deployment:latest .
    fi
fi


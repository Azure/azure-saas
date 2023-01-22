#!/usr/bin/env bash

repo_base="$( git rev-parse --show-toplevel )"
docker_file_folder="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment/act"

docker build --file "${docker_file_folder}/Dockerfile" --tag act-container:latest .
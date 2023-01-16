#!/usr/bin/env bash

repo_base="$( git rev-parse --show-toplevel )"
docker_file_folder="${repo_base}/src/Saas.lib/Deployment.Container"

# redirect to build.sh in the Deployment.Container folder
"${docker_file_folder}/build.sh"
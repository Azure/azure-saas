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


# redirect to build.sh in the Deployment.Container folder
if [[ "${force_update}" == false  ]]; then
    "${docker_file_folder}/build.sh"
else
    "${docker_file_folder}/build.sh" -f
fi

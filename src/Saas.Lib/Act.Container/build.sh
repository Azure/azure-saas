#!/usr/bin/env bash

force_update=false

while getopts 'fn:' flag; do
    case "${flag}" in
        f) 
            force_update=true
            ;;
        n)
            tag_name="$OPTARG"
            ;;
        *) 
            force_update=false
            ;;
    esac
done

# shellcheck disable=SC1091
source "./../constants.sh"

if [[ "${force_update}" == false  ]]; then
    echo "Building the deployment container using the cache. To rebuild use the -f flag."
    docker build --file "${ACT_CONTAINER_DIR}/Dockerfile" --tag "${tag_name}" .
else
    docker build --no-cache --file "${ACT_CONTAINER_DIR}/Dockerfile" --tag "${tag_name}" .
fi

gh extension install https://github.com/nektos/gh-act

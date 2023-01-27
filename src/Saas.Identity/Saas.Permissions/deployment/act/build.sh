#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./../constants.sh"

docker build --file "${ACT_DIR}/Dockerfile" --tag "${ACT_CONTAINER_NAME}" .
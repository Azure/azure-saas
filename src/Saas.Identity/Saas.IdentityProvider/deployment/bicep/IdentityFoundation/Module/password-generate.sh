#!/usr/bin/env bash
pwd="$( openssl rand -base64 48 )"

jq --null-input \
    --arg secret "${pwd}" \
    '{ secret: $secret }' \
    > "${AZ_SCRIPTS_OUTPUT_PATH}"
#!/usr/bin/env bash

function get-same-time-tomorrow() {
    result="$( date -u -d "1 day" +"%Y-%m-%dT%H:%M" )"

    echo "${result}"
    return
}
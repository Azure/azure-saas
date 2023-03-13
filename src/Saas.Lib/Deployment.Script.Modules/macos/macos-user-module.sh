#!/usr/bin/env bash

function get-same-time-tomorrow() {
    result="$( date -u -v +1d  +"%Y-%m-%dT%H:%M" )"

    echo "${result}"
    return
}
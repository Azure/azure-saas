#!/usr/bin/env bash

# Create a random postfix to be used for naming resources.
function get-postfix() {
    post_fix="$( LC_CTYPE=C tr -dc 'a-z0-9' < /dev/urandom | fold -w 4 | head -n 1 )"

    echo "${post_fix}"
    return
}
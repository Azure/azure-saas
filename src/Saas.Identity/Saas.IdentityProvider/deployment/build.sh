#!/usr/bin/env bash

architecture="$( uname -a )"

if [[ "${architecture}" == *"ARM64"* ]]; then
    echo "Building for ARM64 (including Apple Sillicon)..."
    docker build --file Dockerfile.Apple-Silicon -t asdk-idprovider:latest .
else
    docker build --file Dockerfile -t asdk-idprovider:latest .
fi

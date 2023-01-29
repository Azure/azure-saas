#!/usr/bin/env bash

# repo base
repo_base="$( git rev-parse --show-toplevel )"
REPO_BASE="${repo_base}"

# running the './act/script/clean-credentials' script using our ASDK deployment script container - i.e., not the act container
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment/":/asdk/src/Saas.Identity/Saas.Permissions/deployment:ro \
    --volume "${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules/":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config:ro \
    --volume "${REPO_BASE}/.git/":/asdk/.git:ro \
    --volume "${HOME}/.azure/":/asdk/.azure:ro \
    --volume "${HOME}/asdk/act/.secret":/asdk/act/.secret \
    asdk-script-deployment:latest \
    bash /asdk/src/Saas.Identity/Saas.Permissions/deployment/script/clean-credentials.sh

./setup.sh -s
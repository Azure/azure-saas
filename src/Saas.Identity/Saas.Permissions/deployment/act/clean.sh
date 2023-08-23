#!/usr/bin/env bash

# repo base
repo_base="$(git rev-parse --show-toplevel)"
REPO_BASE="${repo_base}"

host_deployment_dir="${repo_base}/src/Saas.Identity/Saas.Permissions/deployment"
container_deployment_dir="/asdk/src/Saas.Identity/Saas.Permissions/deployment"

# running the './act/script/clean-credentials' script using our ASDK deployment script container - i.e., not the act container
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${host_deployment_dir}":"${container_deployment_dir}":ro \
    --volume "${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules/":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config:ro \
    --volume "${REPO_BASE}/.git/":/asdk/.git:ro \
    --volume "${HOME}/.azure/":/asdk/.azure:ro \
    --volume "${HOME}/asdk/act/.secret":/asdk/act/.secret \
    --env "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE"="${container_deployment_dir}" \
    "${DEPLOYMENT_CONTAINER_NAME}" \
    bash /asdk/src/Saas.Lib/Deployment.Script.Modules/clean-credentials.sh

./setup.sh -s

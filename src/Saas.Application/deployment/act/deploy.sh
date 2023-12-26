#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    source "./../constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
}

repo_base="$(git rev-parse --show-toplevel)"
REPO_BASE="${repo_base}"

host_act_secrets_dir="${HOME}/asdk/act/.secret"
host_deployment_dir="${REPO_BASE}/src/Saas.Application/deployment"
container_deployment_dir="/asdk/src/Saas.Application/deployment"

# running the './act/script/patch-app-name.sh' script using our ASDK deployment script container - i.e., not the act container
docker run \
    --platform linux/amd64 \
    --interactive \
    --tty \
    --rm \
    --volume "${host_deployment_dir}":"${container_deployment_dir}":ro \
    --volume "${host_deployment_dir}/act/workflows/":"${container_deployment_dir}/act/workflows" \
    --volume "${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules/":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config:ro \
    --volume "${REPO_BASE}/.git/":/asdk/.git:ro \
    --volume "${HOME}/.azure/":/asdk/.azure:ro \
    --volume "${host_act_secrets_dir}":/asdk/act/.secret \
    --env "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE"="${container_deployment_dir}" \
    "${DEPLOYMENT_CONTAINER_NAME}" \
    bash /asdk/src/Saas.Lib/Deployment.Script.Modules/deploy-debug.sh

# run act container to run github action locally, using local workflow file and local code base.
gh act workflow_dispatch \
    --rm \
    --bind \
    --pull=false \
    --secret-file "${host_act_secrets_dir}/secret" \
    --directory "${REPO_BASE}" \
    --workflows "${ACT_LOCAL_WORKFLOW_DEBUG_FILE}" \
    --platform "ubuntu-latest=${ACT_CONTAINER_NAME}"

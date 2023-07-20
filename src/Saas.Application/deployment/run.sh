#!/usr/bin/env bash

source "./constants.sh"

repo_base="$(git rev-parse --show-toplevel)"
host_act_secrets_dir="${HOME}/asdk/act/.secret"

host_deployment_dir="${repo_base}/src/Saas.Application/deployment"
container_deployment_dir="/asdk/src/Saas.Application/deployment"

# using volumes '--volume' to mount only the needed directories to the container.
# using ':ro' to make scrip directories etc. read-only. Only config and log directories are writable.
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${host_deployment_dir}":"${container_deployment_dir}":ro \
    --volume "${host_deployment_dir}/log":"${container_deployment_dir}/log" \
    --volume "${host_deployment_dir}/bicep/parameters":"${container_deployment_dir}"/bicep/parameters \
    --volume "${repo_base}/src/Saas.Identity/Saas.IdentityProvider/deployment/config":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config \
    --volume "${repo_base}/src/Saas.Lib/Deployment.Script.Modules":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${repo_base}/src/Saas.Lib/Saas.Bicep.Module":/asdk/src/Saas.Lib/Saas.Bicep.Module:ro \
    --volume "${repo_base}/.github/workflows":/asdk/.github/workflows \
    --volume "${repo_base}/.git/":/asdk/.git:ro \
    --volume "${HOME}/.azure/":/asdk/.azure:ro \
    --volume "${host_act_secrets_dir}":/asdk/act/.secret \
    --env "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE"="${container_deployment_dir}" \
    "${DEPLOYMENT_CONTAINER_NAME}" \
    bash ${container_deployment_dir}/start.sh

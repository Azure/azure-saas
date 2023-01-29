#!/usr/bin/env bash

# shellcheck disable=SC1091
{
    source "./../constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
}

host_act_secrets_dir="${HOME}/asdk/act/.secret"

# running the './act/script/patch-app-name.sh' script using our ASDK deployment script container - i.e., not the act container
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment/":/asdk/src/Saas.Identity/Saas.Permissions/deployment:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment/act/workflows/":/asdk/src/Saas.Identity/Saas.Permissions/deployment/act/workflows \
    --volume "${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules/":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config:ro \
    --volume "${REPO_BASE}/.git/":/asdk/.git:ro \
    --volume "${HOME}/.azure/":/asdk/.azure:ro \
    --volume "${host_act_secrets_dir}":/asdk/act/.secret \
    asdk-script-deployment:latest \
    bash /asdk/src/Saas.Identity/Saas.Permissions/deployment/script/deploy-debug.sh

# run act container to run github action locally, using local workflow file and local code base.
gh act workflow_dispatch \
    --rm \
    --bind \
    --secret-file "${host_act_secrets_dir}/secret" \
    --directory "${REPO_BASE}" \
    --workflows "${ACT_PERMISSIONS_DEPLOYMENT_LOCAL_WORKFLOW_FILE}" \
    --platform "ubuntu-latest=${ACT_CONTAINER_NAME}"
    
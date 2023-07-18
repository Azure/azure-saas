#!/usr/bin/env bash

repo_base="$(git rev-parse --show-toplevel)"
git_repo_origin="$(git config --get remote.origin.url)"
git_org_project_name="$(git config --get remote.origin.url | sed 's/.*\/\([^ ]*\/[^.]*\).*/\1/')"
gh_auth_token="$(gh auth token)"

if [[ -z "${gh_auth_token}" ]]; then
    echo "You are not loggged into your GitHub organization. GitHub auth token is not set and/or you haven't installed GitHub Cli."
    echo "Please make sure that GitHub Cli is installed and then run 'gh auth login', before running this script again."
    echo "See readme.md for more info."
    exit 0
fi

# using volumes '--volume' to mount only the needed directories to the container.
# using ':ro' to make scrip directories etc. read-only. Only config and log directories are writable.
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${repo_base}/":/asdk/ \
    --volume "${HOME}/.azure/":/asdk/.azure \
    --volume "${HOME}/asdk/.cache/":/asdk/.cache \
    --env "ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE=/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment" \
    --env "GIT_REPO_ORIGIN=${git_repo_origin}" \
    --env "GIT_ORG_PROJECT_NAME=${git_org_project_name}" \
    --env "GITHUB_AUTH_TOKEN=${gh_auth_token}" \
    asdk-script-deployment:latest \
    bash /asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/start.sh

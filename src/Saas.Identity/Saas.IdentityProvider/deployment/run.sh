#!/usr/bin/env bash

project_dir="$( pwd )"

git_repo_origin="$( git config --get remote.origin.url )"
git_org_project_name="$( git config --get remote.origin.url | sed 's/.*\/\([^ ]*\/[^.]*\).*/\1/' )"
gh_auth_token="$( gh auth token )"

if [[ -z "${gh_auth_token}" ]]; then
    echo "Not loggged into your GitHub organization. GitHub auth token is not set. Please run 'gh auth login' first to set it and then run this script again."
    exit 1
fi

docker run \
    -it \
    --rm \
    -v "${project_dir}":/asdk/Saas.IdentityProvider/deployment \
    -v "${project_dir}/../policies":/asdk/Saas.IdentityProvider/policies \
    -v "${project_dir}/../../SaaS.Identity.IaC":/asdk/SaaS.Identity.IaC \
    -v "${HOME}/.azure/":/asdk/.azure:ro \
    -v "${HOME}/asdk/.cache/":/asdk/.cache \
    -e "GIT_REPO_ORIGIN=${git_repo_origin}" \
    -e "GIT_ORG_PROJECT_NAME=${git_org_project_name}" \
    -e "GITHUB_AUTH_TOKEN=${gh_auth_token}" \
    asdk-idprovider:latest \
    bash start.sh

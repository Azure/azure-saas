#!/usr/bin/env bash

# shellcheck disable=SC1091
source "$SHARED_MODULE_DIR/config-module.sh"
source "$SHARED_MODULE_DIR/log-module.sh"

function environment-create-request() {
    request="$( cat <<-END
{
    "deployment_branch_policy": {
        "protected_branches": False,
        "custom_branch_policies" : True
    }
}
END
)"
    echo "${request}"
    return
}

function create-github-environment() {
    local environment_name=$1

    request="$( environment-create-request )"

    echo "${request}"

    gh api \
        --header "Accept: application/vnd.github+json" \
        --method PUT "/repos/:owner/:repo/environments/${environment_name}" \
        <<< "${request}" \
        | log-output \
            --level info \
            || echo "Failed to create directory in Azure Blob Storage." \
                | log-output \
                    --level error \
                    --header "Critical error"
}

function get-environment-policy() {
    local environment_name=$1

    gh api \
        --header "Accept: application/vnd.github+json" \
        /repos/1iveowl/azure-saas/environments/production/deployment-branch-policies \
         | log-output \
            --level info \
            || echo "Failed to create directory in Azure Blob Storage." \
                | log-output \
                    --level error \
                    --header "Critical error"

    return
}

function get-github-repo() {
    if [ -f /.dockerenv ]; then
        gh auth login --with-token <<< "${GITHUB_AUTH_TOKEN}"

        # shellcheck disable=SC2153
        git_repo_origin="${GIT_REPO_ORIGIN}"
    else
        has_token="$( gh auth token --show-token )"

        if [[ -z "${has_token}" ]]; then
            echo "You must be logged into GitHub to continue." \
                | log-output \
                    --level info

            gh auth login \
                || echo "Failed to login to GitHub." \
                    | log-output \
                        --level error \
                        --header "Critical error" \
                        || exit 1
        fi

        git_repo_origin="$( git config --get remote.origin.url )"
    fi

    echo "${git_repo_origin}"
    return
}

function set-github-secret() {
    local secret_name=$1
    local secret_value=$2
    local git_repo_origin=$3
    local environment=$4

    if [[ -n $environment ]]; then
        gh secret set "${secret_name}" \
            --env "${environment}" \
            --body "${secret_value}" \
            --repo "${git_repo_origin}" \
            || echo "Failed to set secret with name '${secret_name}' for environment '${environment}'." \
                | log-output \
                    --level error \
                    --header "Critical error"
    else
        gh secret set "${secret_name}" \
            --body "${secret_value}" \
            --repo "${git_repo_origin}" \
            || echo "Failed to set secret with name '${secret_name}'." \
                | log-output \
                    --level error \
                    --header "Critical error"
    fi
}
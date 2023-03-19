#!/usr/bin/env bash

# work-around for issue w/ az cli bicep v. 2.46: https://github.com/Azure/azure-cli/issues/25710
az config set bicep.use_binary_from_path=false

# if not running in a container
if ! [ -f /.dockerenv ]; then
    echo "Running outside of a container us not supported. Please run the deployment script using './run.sh'."
    exit 0
fi

# shellcheck disable=SC1091
{
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/user-module.sh"
}

# set bash options to exit on unset variables and errors (exit 1) including pipefail
set -u -e -o pipefail

if ! [[ -f $CONFIG_FILE ]]; then
    echo "The ASDK Identity Foundation has not completed or 'config.json' file from it's deployment is missing. Please run the Identity Foundation deployment script first."
    exit 0
fi

# get now date and time for backup file name
now=$(date '+%Y-%m-%d--%H-%M-%S')

# set run time for deployment script instance
export ASDK_DEPLOYMENT_SCRIPT_RUN_TIME="${now}"

# using the az cli settings and cache from the host machine
initialize-az-cli "$HOME/.azure"

echo "Provisioning the SaaS Signup Administration web app..." |
    log-output \
        --level info \
        --header "SaaS Signup Administration"

"${SHARED_MODULE_DIR}/"deploy-app-service.sh

"${SHARED_MODULE_DIR}/"deploy-config-entries.sh

echo "Patching '${APP_NAME}' GitHub Action workflow file." |
    log-output \
        --level info \
        --header "SaaS Sign-up Administration Web App"

"${SHARED_MODULE_DIR}/patch-github-workflow.py" \
    "${APP_NAME}" \
    "${CONFIG_FILE}" \
    "${GITHUB_ACTION_WORKFLOW_FILE}" ||
    echo "Failed to patch ${APP_NAME} GitHub Action workflow file" |
    log-output \
        --level error \
        --header "Critical Error" ||
    exit 1

git_repo_origin="$(git config --get remote.origin.url)"

echo "'${APP_NAME}' is ready to be deployed. You have two options:"
echo "    a) To deploy to production, use the GitHub Action: ${git_repo_origin::-4}/actions"
echo
echo "    b) To deploy for live debugging in Azure; navigate to the act directory ('cd act') and run './setup.sh' and then run './deploy.sh' to deploy for remote debugging."

#!/usr/bin/env bash

# shellcheck disable=SC1091
{
    source "./../constants.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
}

# running the './act/start.sh' script using our ASDK deployment script container - i.e., not the act container
docker run \
    --interactive \
    --tty \
    --rm \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment/":/asdk/src/Saas.Identity/Saas.Permissions/deployment:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.Permissions/deployment/act/workflows/":/asdk/src/Saas.Identity/Saas.Permissions/deployment/act/workflows \
    --volume "${REPO_BASE}/src/Saas.Lib/Deployment.Script.Modules/":/asdk/src/Saas.Lib/Deployment.Script.Modules:ro \
    --volume "${REPO_BASE}/src/Saas.Identity/Saas.IdentityProvider/deployment/config/":/asdk/src/Saas.Identity/Saas.IdentityProvider/deployment/config:ro \
    --volume "${REPO_BASE}/.git/":/asdk/.git:ro \
    asdk-script-deployment:latest \
    bash /asdk/src/Saas.Identity/Saas.Permissions/deployment/act/patch-app-name.sh

declare secret

# Creating a new secret for accessing the OIDC app access deployment.
if  [[ ! -f "${SECRETS_FILE}" || ! -s "${SECRETS_FILE}" ]]; then
     oidc_app_name="$( get-value ".oidc.name" )"
     oidc_app_id="$( get-value ".oidc.appId" )"

    echo "Creating the OIDC app '${oidc_app_name}'"

    touch "${SECRETS_FILE}"
    
    # getting app crentials
    secret_json="$( az ad app credential reset \
        --id "${oidc_app_id}" \
        --display-name "secret-${oidc_app_name}" \
        --query "{clientId:appId, clientSecret:password, tenantId:tenant}" \
        2> /dev/null )"

    subscription_id="$( get-value ".initConfig.subscriptionId" )"

    # adding subscription id to secret json
    secret_json="$( jq --arg sub_id "${subscription_id}" \
        '. | .subscriptionId = $sub_id' \
        <<< "${secret_json}" )"

    # outputting secret json to file in compat format with escape double quotes 
    secret="$( echo "${secret_json}" \
        | jq --compact-output '.' \
        | sed 's/"/\\"/g' )"

    secret="AZURE_CREDENTIALS=\"${secret}\""

    echo "${secret}" > "${SECRETS_FILE}"

    echo "Waiting for 30 seconds to allow new secret to be propagated."
    sleep 30
else
    echo "Using existing secrets file at: '${SECRETS_FILE}'. If you want to create a new one, please delete the existing file and run '/.setup.sh' again and then run './run.sh'."
fi

# run act container to run github action locally, using local workflow file and local code base.
gh act workflow_dispatch \
    --rm \
    --bind \
    --secret-file "${SECRETS_FILE}" \
    --directory "${REPO_BASE}" \
    --workflows "${ACT_PERMISSIONS_DEPLOYMENT_LOCAL_WORKFLOW_FILE}" \
    --platform "ubuntu-latest=${ACT_CONTAINER_NAME}"
    
#!/usr/bin/env bash

set -u -e -o pipefail

# shellcheck disable=SC1091
{
    # include script modules into current shell
    source "${ASDK_DEPLOYMENT_SCRIPT_PROJECT_BASE}/constants.sh"
    source "$SHARED_MODULE_DIR/log-module.sh"
    source "$SHARED_MODULE_DIR/config-module.sh"
    source "$SHARED_MODULE_DIR/app-reg-module.sh"
    source "$SHARED_MODULE_DIR/service-principal-module.sh"
    source "$SHARED_MODULE_DIR/oidc-workflow-module.sh"
    source "$SHARED_MODULE_DIR/github-module.sh"
}

# For more about what's going on here see: https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=openid

oidc_workflow_name="$( get-value ".oidc.name" )"

 echo "Provisioning OIDC Workflow and federation for GitHub Actions '${oidc_workflow_name}'.." \
    | log-output \
        --level info \
        --header "OpenID Connect Workflow"

oidc_app_id="$( get-value ".oidc.appId" )"
oidc_app_obj_id="$( get-value ".oidc.objectId" )"
  
# create an OIDC Connect Workflow App
if ! app-exist "${oidc_app_id}"; then
    echo "Creating OIDC Connect Workflow app '${oidc_workflow_name}'..." \
        | log-output \
            --level info

    oidc_workflow_app_json="$( az ad app create \
        --display-name "${oidc_workflow_name}" \
        --only-show-errors \
        --query "{Id:id, AppId:appId}" \
        || echo "Failed to create OIDC Connect Workflow app." \
            | log-output \
                --level error \
                --header "Critical error" \
                || exit 1 )"

    echo "OICD Connect Workflow app created: ${oidc_workflow_app_json}" \
        | log-output \
            --level info

    oidc_app_obj_id=$( jq --raw-output '.Id'  <<< "${oidc_workflow_app_json}" )
    oidc_app_id=$( jq --raw-output '.AppId'   <<< "${oidc_workflow_app_json}" )

    put-value ".oidc.objectId" "${oidc_app_obj_id}"
    put-value ".oidc.appId" "${oidc_app_id}"
fi

assignee_obj_id="$( get-value ".oidc.assigneeObjectId" )"

if ! service-principal-exist-by-id "${assignee_obj_id}"; then
    echo "Creating Service Principal for '${oidc_workflow_name}' with appId '${oidc_app_id}'..." \
        | log-output \
            --level info

    # create an OIDC Connect Workflow service principal
    assignee_obj_id="$( az ad sp create \
        --id "${oidc_app_id}" \
        --query id \
        --output tsv \
        || echo "Failed to create OIDC Connect Workflow service principal." \
            | log-output \
                --level error \
                --header "Critical error" \
                || exit 1 )"

    put-value ".oidc.assigneeObjectId" "${assignee_obj_id}"
fi

resource_group_name="$( get-value ".deployment.resourceGroup.name" )"

if ! role-assignment-exist  "${resource_group_name}" "${assignee_obj_id}"; then
    subscription_id="$( get-value ".initConfig.subscriptionId" )"    
    scope="/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}"
    
    echo "Creating Role Assignment..." \
    | log-output \
        --level info

    echo "Using Resource Group scope '${scope}'." \
    | log-output \
        --level info

    echo "Using assignee object id '${assignee_obj_id}'." \
    | log-output \
        --level info

    az role assignment create \
        --role "Contributor" \
        --assignee-object-id "${assignee_obj_id}" \
        --scope "${scope}" \
        --assignee-principal-type ServicePrincipal \
            | log-output \
                --level info \
        || echo "Failed to create OIDC Connect Workflow role assignment." \
            | log-output \
                --level error \
                --header "Critical error" \
                || exit 1
fi

git_org_project_name="$( get-value ".git.orgProjectName" )"

subject="repo:${git_org_project_name}:ref:refs/heads/main"

put-value ".oidc.credentials.subject" "${subject}"

if ! federation-exist "${oidc_app_id}" "${subject}" ; then
    echo "Creating OIDC Connect Workflow federation..." \
        | log-output \
            --level info

    credential_json="$( get-object ".oidc.credentials" )"

    echo "Using credentials '${credential_json}'." \
        | log-output \
            --level info

    federation_id="$( az ad app federated-credential create \
        --id "${oidc_app_obj_id}" \
        --parameters "${credential_json}" \
        --query id \
        --output tsv )" \
        || echo "Failed to create OIDC Connect Workflow federation." \
            | log-output \
                --level error \
                --header "Critical error" \
                || exit 1

    put-value ".oidc.federation.id" "${federation_id}"
fi

tenant_id="$( get-value ".initConfig.tenantId" )"
subscription_id="$( get-value ".initConfig.subscriptionId" )"

git_repo_origin="$( get-github-repo )" \
    || echo "Failed to get GitHub repo." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1

echo "Setting GitHub secrets for OIDC Workflow..." \
    | log-output \
        --level info

(
    set-github-secret \
        "AZURE_CLIENT_ID" \
        "${oidc_app_id}" \
        "${git_repo_origin}" \
        ""

    set-github-secret \
        "AZURE_TENANT_ID" \
        "${tenant_id}" \
        "${git_repo_origin}" \
        ""

    set-github-secret \
        "AZURE_SUBSCRIPTION_ID" \
        "${subscription_id}" \
        "${git_repo_origin}" \
        ""
) \
    || echo "Failed to set GitHub secrets for OIDC Workflow." \
        | log-output \
            --level error \
            --header "Critical error" \
            || exit 1
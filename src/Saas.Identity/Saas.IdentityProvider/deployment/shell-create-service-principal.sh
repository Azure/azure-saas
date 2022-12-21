#!/bin/bash

set -u -e -o pipefail

# include script modules into current shell
source "./script/config-module.sh"
source "./script/service-principal-module.sh"

service_principal_name="$( get-value ".deployment.azureb2c.servicePrincipal.username" )"

echo "Provisioning or updating service principal for policy key creation: ${service_principal_name}" \
    | log-output \
        --level info \
        --header "Provisioning Service Principal for Policy Key Creation"

# create temporary service principal for adding the policy keys for the B2C tenant before exiting az cli b2c session
create-service-principal-for-policy-key-creation "${service_principal_name}"

echo "Service principal created successfully. Waiting 30 seconds to let it propagate..." \
    | log-output \
        --level success

sleep 30s
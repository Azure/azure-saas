#!/bin/bash

set -u -e -o pipefail

# include script modules into current shell
source "./script/config-module.sh"
source "./script/log-module.sh"
source "./script/service-principal-module.sh"

app_id="$( get-value ".deployment.azureb2c.servicePrincipal.appId" )"

delete-service-principal "${app_id}"

echo "Service principal deletion has completed." | log-output --level success
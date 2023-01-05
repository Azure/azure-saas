#!/usr/bin/env bash

project_dir="$( dirname "$( readlink -f "$0" )" )"

docker run \
    -it \
    --rm \
    -v "${project_dir}":/asdk/Saas.IdentityProvider/deployment \
    -v "${project_dir}/../policies":/asdk/Saas.IdentityProvider/policies \
    -v "${project_dir}/../../SaaS.Identity.IaC":/asdk/SaaS.Identity.IaC \
    asdk-idprovider:latest \
    bash start.sh

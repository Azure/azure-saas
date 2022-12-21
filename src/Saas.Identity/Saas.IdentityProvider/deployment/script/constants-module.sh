#!/bin/bash

BASEDIR="$( dirname "$( readlink -f "$0" )" )"
CONFIG_FILE="${BASEDIR}/config/config.json"
CONFIG_TEMPLATE_FILE="${BASEDIR}/config/config-template.json"
LOG_FILE_DIR="${BASEDIR}/log"
LOG_FILE="${BASEDIR}/log/deploy.log"
PUBLIC_KEY="${BASEDIR}/certs/public-key.pem"
CERTIFICATE_POLICY_FILE="${BASEDIR}/config/certificate-policy.json"
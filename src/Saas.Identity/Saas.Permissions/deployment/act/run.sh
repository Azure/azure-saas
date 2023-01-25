#!/usr/bin/env bash

gh act workflow_dispatch --secret-file .secrets -W ./workflows/permissions-api-deploy-debug.yml -P ubuntu-latest=act-container:latest
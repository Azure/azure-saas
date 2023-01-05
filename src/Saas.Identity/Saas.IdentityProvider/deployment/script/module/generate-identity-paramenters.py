#!/usr/bin/env python3

import json
import sys

def get_app_value(config: dict, app_name: str, key: str) -> str:
    for item in config['appRegistrations']: 
        if item['name'] == app_name: return item[key]

def patch_paramenters_file(config_file: str, paramenter_file: str) -> None:
    with open(config_file, 'r') as f:
        config = json.load(f)

    with open(paramenter_file, 'r') as f:
        parameters = json.load(f)

    parameters['parameters']['appSettingsAdminServiceBaseUrl']['value'] \
        = get_app_value(config, "admin-api", "baseUrl")
    
    parameters['parameters']['solutionPostfix']['value'] \
        = config['deployment']['postfix']

    parameters['parameters']['solutionPrefix']['value'] \
        = config['initConfig']['naming']['solutionPrefix']

    parameters['parameters']['solutionName']['value'] \
        = config['initConfig']['naming']['solutionName']

    parameters['parameters']['keyVaultName']['value'] \
        = config['deployment']['keyVault']['name']
    
    parameters['parameters']['azureB2CDomain']['value'] \
        = config['deployment']['azureb2c']['domainName']

    parameters['parameters']['azureB2CLoginEndpoint']['value'] \
        = f"https://{config['deployment']['azureb2c']['name']}.b2clogin.com"

    parameters['parameters']['azureB2CTenantId']['value'] \
        = config['deployment']['azureb2c']['tenantId']

    parameters['parameters']['azureB2CPermissionApiClientId']['value'] \
        = get_app_value(config, "permissions-api", "appId")

    parameters['parameters']['permissionsApiName']['value'] \
        = get_app_value(config, "permissions-api", "apiName")

    parameters['parameters']['sqlAdministratorLogin']['value'] \
        = config['sql']['sqlAdminLoginName']

    parameters['parameters']['containerRegistryUrl']['value'] \
            = config['container']['containerRegistryUrl']

    parameters['parameters']['permissionsApiContainerImageTag']['value'] \
            = config['container']['permissionsApiContainerImageTag']

    with open(paramenter_file, 'w') as f:
        f.write(json.dumps(parameters, indent=4))

# Main entry point for the script 
config_file = sys.argv[1]
paramenter_file = sys.argv[2]

patch_paramenters_file(config_file, paramenter_file)
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

    parameters['parameters']['version']['value'] \
        = config['version']

    parameters['parameters']['environment']['value'] \
        = config['environment']

    parameters['parameters']['devMachineIp']['value'] \
        = config['deployment']['devMachine']['ip']
    
    parameters['parameters']['solutionPostfix']['value'] \
        = config['deployment']['postfix']

    parameters['parameters']['solutionPrefix']['value'] \
        = config['initConfig']['naming']['solutionPrefix']

    parameters['parameters']['solutionName']['value'] \
        = config['initConfig']['naming']['solutionName']

    parameters['parameters']['keyVaultName']['value'] \
        = config['deployment']['keyVault']['name']
    
    parameters['parameters']['permissionApiKey']['value'] \
        = 'RestApiKey'

    parameters['parameters']['sqlAdministratorLogin']['value'] \
        = config['sql']['sqlAdminLoginName']

    with open(paramenter_file, 'w+') as f:
        f.write(json.dumps(parameters, indent=4))

# Main entry point for the script 
if __name__ == "__main__":
    config_file = sys.argv[1]
    paramenter_file = sys.argv[2]

    patch_paramenters_file(config_file, paramenter_file)
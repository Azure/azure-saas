#!/usr/bin/env python3

import json
import sys
import re

def get_app_value(config: dict, app_name: str, key: str) -> str:
    for item in config['appRegistrations']: 
        if item['name'] == app_name: return item[key]

def get_output_value(outputs: dict, output_name: str) -> 'dict[str, dict[str, str]]':
    item = outputs[output_name]
    if item : return {
        output_name : {
            'value': item['value']
        }
    }

def patch_paramenters_file(
        app_name: str, 
        identity_outputs: str, 
        paramenter_file: str,
        config_file: str) -> None:

    with open(config_file, 'r') as f:
        config = json.load(f)

    with open(identity_outputs, 'r') as f:
        identity_outputs = json.load(f)

    with open(paramenter_file, 'r') as f:
        parameters = json.load(f)

    print(f"App Name: {app_name}")

    app_service_name = get_app_value(config, app_name, 'appServiceName')

    app_key_name = re.sub('-', '', app_name)

    parameters['parameters'].update({
        app_key_name: {
            'value': app_service_name
            }
    })

    parameters['parameters'].update(get_output_value(identity_outputs, 'version'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'environment'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'appServicePlanName'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'keyVaultUri'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'location'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'userAssignedIdentityName'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'appConfigurationName'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'logAnalyticsWorkspaceName'))
    parameters['parameters'].update(get_output_value(identity_outputs, 'applicationInsightsName'))

    with open(paramenter_file, 'w') as f:
        f.write(json.dumps(parameters, indent=4))


# Main entry point for the script 
if __name__ == "__main__":
    app_name = sys.argv[1]
    identity_outputs = sys.argv[2]
    paramenter_file = sys.argv[3]
    config_file = sys.argv[4]

    patch_paramenters_file(app_name, identity_outputs, paramenter_file, config_file)
#!/usr/bin/env python3

import sys
from ruamel.yaml import YAML
import json

def get_app_value(config: dict, app_name: str, key: str) -> str:
    for item in config['appRegistrations']: 
        if item['name'] == app_name: return item[key]

def patch_workflow(config_file: str, workflow_yaml: str) -> None:
    with open(config_file, 'r') as f_json:
        config_json = json.load(f_json)

        web_app_name = get_app_value(config_json, "permissions-api", "apiName")

        print(f"Web app name: {web_app_name}")

    yaml=YAML()

    yaml.preserve_quotes = True
    yaml.indent(mapping=2, sequence=4, offset=2)
    yaml.explicit_start = True

    with open(workflow, 'r') as stream:
        workflow_yaml = yaml.load(stream)

        env = workflow_yaml['env']

        env.update(dict(AZURE_WEBAPP_NAME=web_app_name))

        # print(f"Workflow: {workflow_yaml['env']['AZURE_WEBAPP_NAME']}")

        workflow_yaml['env']['AZURE_WEBAPP_NAME'] = web_app_name

    with open(workflow, 'w+') as stream:
        yaml.dump(workflow_yaml, stream)

# Main entry point for the script 
if __name__ == "__main__":
    config_file = sys.argv[1]
    workflow = sys.argv[2]
    patch_workflow(config_file, workflow)
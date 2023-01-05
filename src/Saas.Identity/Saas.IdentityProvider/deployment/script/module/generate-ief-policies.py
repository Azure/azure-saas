#!/usr/bin/env python3

import json
import sys
import re
import os
import shutil
from pathlib import Path
from model import AppSettings

def get_app_value(config: dict, app_name: str, key: str) -> str:
    for item in config['appRegistrations']: 
        if item['name'] == app_name: return item[key]

def create_appsettings_file(config_file: str, app_settings_file: str) -> None:
    # Read the config manifest file
    with open(config_file, 'r') as f:
        config = json.load(f)

    # get the values to be added to the appsettings file
    name = config['environment']
    production = config['production']
    tenant = config['deployment']['azureb2c']['domainName']
    identityExperienceFrameworkAppId = get_app_value(config, "IdenityExperienceFramework", "appId")
    proxyIdentityExperienceFrameworkAppId = get_app_value(config, "ProxyIdentityExperienceFramwork", "appId")
    permissionsAPIUrl = get_app_value(config, "permissions-api", "permissionsApiUrl")
    rolesAPIUrl = get_app_value(config, "permissions-api", "rolesApiUrl")
    restAPIKey = "B2C_1A_" + "RestApiKey"

    # create the policySettings part of the appSettings json file
    policySettings = AppSettings.PolicySettings(
        identityExperienceFrameworkAppId, 
        proxyIdentityExperienceFrameworkAppId, 
        permissionsAPIUrl, 
        rolesAPIUrl,
        restAPIKey)
    
    # add the environment part of the appSettings json file
    environment = AppSettings.Environment(name, production, tenant, policySettings)
    appsettings = AppSettings.Appsettings([environment])

    # write the appsettings json file to the specificied file name and directory
    # overwrite if it exists
    with open(app_settings_file, 'w') as f:
        f.write(appsettings.toJson())

# https://learn.microsoft.com/en-us/azure/active-directory-b2c/trustframeworkpolicy
def patch_policy_file(directory: str, settings_name: str, replace_str: str) -> None:
    # Get all the policy xml files (.xml) in the directory
    files = Path(directory).glob('*.xml')

    # Regex pattern to match the settings_name
    regex_pattern = fr'{{Settings:{settings_name}}}'

    # Patch the policy files with value corresponding to the settings_name
    for xmlfile in files:
        with open(xmlfile, 'r') as xmlin:
            doc = xmlin.read()
            patched_doc = re.sub(regex_pattern, replace_str, doc, flags=re.IGNORECASE)
        with open(xmlfile, 'w') as xmlout:
                xmlout.write(patched_doc)

def copy_policy_files(directory: str, output_dir: str) -> None:
    # Create the output directory if it doesn't exist
    if not os.path.exists(output_dir):
            os.makedirs(output_dir)

    # Copy the policy xml files (.xml) to the output directory
    files = Path(directory).glob('*.xml')

    for xmlfile in files:
        shutil.copy(xmlfile, os.path.join(output_dir, xmlfile.name))

def policy_builder(policy_dir: str, app_settings_file: str) -> None:
    with open(app_settings_file, 'r') as f:
        app_str= f.read()
        appsettingsDict = json.loads(app_str)

    # For each environment in the appsettings file create separate folder with the policy files
    for env in appsettingsDict['Environments']:
        tenantId = env['Tenant']
        environment = env['Name']

        output_dir = os.path.join(policy_dir, "Environments/" + environment)

        # Copy the policy files to the output directory
        copy_policy_files(policy_dir, output_dir)

        # Patch the policy files with the tenantId
        patch_policy_file(output_dir, "Tenant", tenantId)
        
        # Patch the policy files with each of the policies settings
        for policyName in env['PolicySettings']:
            val = env['PolicySettings'][policyName]
            patch_policy_file (output_dir, policyName, val)

def main(config_file: str, app_settings_file: str, policy_dir: str) -> None:
    create_appsettings_file(config_file, app_settings_file)
    policy_builder(policy_dir, app_settings_file)


# Main entry point for the script 

# config/json file containing the manifest 
config_file = sys.argv[1]

# appsettings file to be generated
app_settings_file = sys.argv[2]

# policy directory containing the xml policy files
policy_dir = sys.argv[3]

# Call the main function
main(config_file, app_settings_file, policy_dir)




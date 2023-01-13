#!/usr/bin/env python3

import json
import sys

empty_parameters_file = '../config/empty-bicep-parameters-file.json'

def map_to_parameters_file(bicep_output_file: str, parameters_file: str):
    with open(bicep_output_file, 'r') as f:
        bicep_output = json.load(f)

    with open (empty_parameters_file, 'r') as f:
        parameters = json.load(f)

    for bo in bicep_output:
        parameters['parameters'][bo] = {}
        parameters['parameters'][bo]['value'] = bicep_output[bo]['value']

    with open(parameters_file, 'w+') as f:
        f.write(json.dumps(parameters, indent=4))

# Main entry point for the script
if __name__ == "__main__":
    bicep_output_file = sys.argv[1]
    parameters_file = sys.argv[2]
    map_to_parameters_file(bicep_output_file, parameters_file)
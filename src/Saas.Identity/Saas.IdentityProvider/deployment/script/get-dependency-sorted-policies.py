#!/usr/bin/env python3

import sys
import json
from pathlib import Path
import xml.etree.ElementTree as ET
import os

def get_policy_id(xml_file: str) -> str:
    tree = ET.parse(xml_file)
    root = tree.getroot()
    return root.attrib['PolicyId']

def get_depend_on(xml_file: str) -> str:
    # Policy files rely on a default namespace
    # https://docs.python.org/3.5/library/xml.etree.elementtree.html#parsing-xml-with-namespaces
    namespaces = {'default':'http://schemas.microsoft.com/online/cpim/schemas/2013/06'}

    root = ET.parse(xml_file).getroot()

    policy_id = root.find('./default:BasePolicy/default:PolicyId', namespaces)

    return policy_id.text if policy_id is not None else None

def get_policy_list(policy_dir: str) -> dict:
    # Get all the policy xml files (.xml) in the directory
    files = Path(policy_dir).glob('*.xml')

    policy_dict = {}

    for xmlfile in files:
        id = get_policy_id(xmlfile)
        depend_on = get_depend_on(xmlfile)
        path=os.path.abspath(xmlfile)
        policy_dict[id] = (depend_on, path)

    return policy_dict

def get_dependency_sorted_dict(dict: dict) -> dict:
    sorted_policy_dict = {}

    # start with all the policies that don't depend on anything
    dependecy_list = [None]

    while len(dependecy_list) > 0:
        matching_dict = {}
        for id, (depend_on, xmlfile) in dict.items():
            if depend_on in dependecy_list:
                matching_dict[id] = (depend_on, xmlfile)
        for id, (depend_on, xmlfile) in matching_dict.items():
            # reset dependecy_list
            dependecy_list.clear()
            # add the found dependecy to the dependecy_list of next level
            dependecy_list.append(id)
            # add found policy to sorted dict
            sorted_policy_dict[id] = xmlfile
        if len(matching_dict) == 0:
            # no more policies found, break
            break

    if len(sorted_policy_dict) != len(dict):
        print(f"Found {len(sorted_policy_dict)} policies in total of {len(dict)} policies.")
        print("The following policies are not sorted:")
        for id, depend_on in dict.items():
            if id not in sorted_policy_dict:
                print(f"{id} depends on {depend_on}")
        raise Exception("Not all policies are sorted.")

    return sorted_policy_dict

if __name__ == "__main__":
    policy_dir = sys.argv[1]
    unsorted_policy_dict = get_policy_list(policy_dir)
    sorted_policy_dict = get_dependency_sorted_dict(unsorted_policy_dict)
    arr = [{'id': key, 'path': path } for key, path in sorted_policy_dict.items()]

    print(json.dumps(arr, indent = 4))

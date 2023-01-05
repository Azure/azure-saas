#!/usr/bin/env python3

import sys
import xml.etree.ElementTree as ET

def get_policy_id(xml_file: str) -> str:
    tree = ET.parse(xml_file)
    root = tree.getroot()
    return root.attrib['PolicyId']

xml_file = sys.argv[1]

print(get_policy_id(xml_file))

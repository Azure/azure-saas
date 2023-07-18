import json

class ConfigJson(object):
    def __init__(self, config: dict):
        self.config = config

def get_app_value(config: dict, app_name: str, key: str) -> str:
    for item in config['appRegistrations']: 
        if item['name'] == app_name: return item[key]

def get_tenant(config: dict) -> str:
    return config['deployment']['azureb2c']['tenant']
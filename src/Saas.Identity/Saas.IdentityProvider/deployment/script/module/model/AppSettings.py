import json
from json import JSONEncoder

# https://github.com/changsin/Medium/blob/main/notebooks/JSON_Serialization.ipynb

class PolicySettings(object):
    def __init__(self,
            identityExperienceFrameworkAppId: str,
            proxyIdentityExperienceFrameworkAppId: str,
            permissionsAPIUrl: str,
            rolesAPIUrl: str,
            RESTAPIKey: str):
        self.IdentityExperienceFrameworkAppId = identityExperienceFrameworkAppId
        self.ProxyIdentityExperienceFrameworkAppId = proxyIdentityExperienceFrameworkAppId
        self.PermissionsAPIUrl = permissionsAPIUrl
        self.RolesAPIUrl = rolesAPIUrl
        self.RESTAPIKey = RESTAPIKey

    def __iter__(self):
        yield from {
            "IdentityExperienceFrameworkAppId": self.IdentityExperienceFrameworkAppId,
            "ProxyIdentityExperienceFrameworkAppId": self.ProxyIdentityExperienceFrameworkAppId,
            "PermissionsAPIUrl": self.PermissionsAPIUrl,
            "RolesAPIUrl": self.RolesAPIUrl,
            "RESTAPIKey": self.RestApiStorageReference
        }.items

    def __str__(self):
        return json.dumps(dict(self), ensure_ascii=False)
    
    def toJson(self):
        return self.__str__()

class Environment(object):
    def __init__(self, 
            name: str, 
            production: bool, 
            tenant: str, 
            policySettings: PolicySettings):
        self.Name = name
        self.Production = production
        self.Tenant = tenant
        self.PolicySettings = policySettings

    def __iter__(self):
        yield from {
            "Name": self.Name,
            "Production": self.Production,
            "Tenant": self.Tenant,
            "PolicySettings": self.Policies
        }.items
    
    def __str__(self):
        return json.dumps(dict(self), ensure_ascii=False)

    def toJson(self):
        return self.__str__()

class Appsettings(object):
    def __init__(self, environments: list, *args, **kwargs):
        self.Environments = environments

    def __iter__(self):
        yield from {
            "Environments": self.Environments,
        }.items

    def __repr__(self):
        return self.__str__()

    def __str__(self):
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=False, indent=4)

    def toJson(self):
        return self.__str__()

class AppSettingsEncoder(JSONEncoder):
    def default(self, o):
        return o.__dict__

def get_environment(config: dict) -> str:
    return config['environment']

def get_production(config: dict) -> bool:
    return config['production']

def deep_get(d, keys):
    if not keys or d is None:
        return d
    return deep_get(d.get(keys[0]), keys[1:])
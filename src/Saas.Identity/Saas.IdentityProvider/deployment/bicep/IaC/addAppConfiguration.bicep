@description('The name of the Azure App Configuration.')
param appConfigurationName string

@description('The name of the Key Vault.')
param keyVaultName string

@description('The name of the key to hold the value.')
param keyName string

@description('The name of value to store.')
param value string

@description('Is the config value a secret, in which case it must be stored in the Key Vault.')
param isSecret bool = false

@description('Azure App Configuration User Assigned Identity Name.')
param appConfigUserAssignedIdentityName string

var rolesJson = loadJsonContent('../roles.json')
var roles = rolesJson.roles
var keyVaultSecretUser = 'Key Vault Secrets User'

resource appConfigUserAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: appConfigUserAssignedIdentityName
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = if (isSecret){
  name: keyVaultName
}

resource keyVaultEntry 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = if (isSecret) {
  parent: keyVault
  name: replace(keyName, ':', '-')
  properties: {
    value: value
  }
}

resource appConfigurationEntry 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' =  {
  parent:appConfig
    name: replace(keyName, ':', '__')
    properties: {
      value: (!isSecret) ?  value : '{"uri":"${keyVaultEntry.properties.secretUri}"}'
      contentType: (!isSecret) ? 'application/json' : 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
    }
  } 

resource managedIdentityCanReadNotificationSecret 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (isSecret) {
  name: guid(roles[keyVaultSecretUser], appConfigUserAssignedIdentity.id, keyVaultEntry.id)
  scope: keyVaultEntry
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[keyVaultSecretUser])
    principalId: appConfigUserAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

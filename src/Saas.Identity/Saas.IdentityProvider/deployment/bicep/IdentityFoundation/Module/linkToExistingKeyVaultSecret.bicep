
@description('label')
param label string

@description('The name of the Azure App Configuration.')
param appConfigurationName string

@description('The name of the Azure Key Vault.')
param keyVaultName string

@description('Azure App Configuration User Assigned Identity Name.')
param userAssignedIdentityName string

@description('The name of the key in Azure Key Vault.')
param keyVaultKeyName string

@description('The name of the key to hold the value in Azure App Configuration.')
param keyName string

var rolesJson = loadJsonContent('roles.json')
var roles = rolesJson.roles
var keyVaultSecretUser = 'Key Vault Secrets User'

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing =  {
  name: keyVaultName
}

resource keyVaultEntry 'Microsoft.KeyVault/vaults/secrets@2022-07-01' existing = {
  parent: keyVault
  name: replace(keyVaultKeyName, ':', '-')
}

resource appConfigurationEntry 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = {
  parent:appConfig
    name: '${keyName}$ver${label}'
    properties: {
      value: '{"uri":"${keyVaultEntry.properties.secretUri}"}'
      contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
    }
  }

resource managedIdentityCanReadNotificationSecret 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[keyVaultSecretUser], userAssignedIdentity.id, keyVaultEntry.id)
  scope: keyVaultEntry
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[keyVaultSecretUser])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

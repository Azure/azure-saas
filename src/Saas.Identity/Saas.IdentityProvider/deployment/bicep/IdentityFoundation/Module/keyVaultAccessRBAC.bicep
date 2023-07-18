@description('The name of the Key Vault.')
param keyVaultName string

@description('The Principal Id of the Permissions Api User Assigned Managed Identity.')
param userAssignedIdentityName string

var rolesJson = loadJsonContent('./roles.json')
var roles = rolesJson.roles

var keyVaultReader = 'Key Vault Reader'
var keyVaultSecretUser = 'Key Vault Secrets User'
var keyVaultCryptoUser = 'Key Vault Crypto User'

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource managedIdentityCanReadFromKeyVault 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  name: guid(roles[keyVaultReader], userAssignedIdentity.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[keyVaultReader])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource managedIdentityCanReadSecretFromKeyVault 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  name: guid(roles[keyVaultSecretUser], userAssignedIdentity.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[keyVaultSecretUser])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource managedIdentityCanReadPerformCryptoOperationsFromKeyVault 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  name: guid(roles[keyVaultCryptoUser], userAssignedIdentity.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[keyVaultCryptoUser])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

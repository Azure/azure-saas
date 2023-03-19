param storageAccountName string
param userPrincipalId string
param storageContainerName string
param location string = resourceGroup().location

var rolesJson = loadJsonContent('./IdentityFoundation/Module/roles.json')
var roles = rolesJson.roles
var roleStorageBlobDataContributor = 'Storage Blob Data Contributor'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    isHnsEnabled: true
  }

  resource container 'blobServices@2022-09-01' = {
    name: 'default'

    resource container 'containers@2022-09-01' = {
      name: storageContainerName
    }
  }
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[roleStorageBlobDataContributor], userPrincipalId, storageAccount.id)
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleStorageBlobDataContributor])
    principalId: userPrincipalId
    principalType: 'User'
  }
}

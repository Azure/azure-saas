@description('The location for all resources.')
param location string = resourceGroup().location

@description('Configuration Store object with array of key values to be stored.')
param configStore object = {
  appConfigurationName: 'appConfigurationName'
  keyVaultName: 'keyVaultName'
  userAssignedIdentityName: 'userAssignedIdentityName'
  label: 'label'
  entries: [
    {
      key: 'keyName'
      value: 'value'
      isSecret: 'isSecret'
    }
  ]
}

var rolesJson = loadJsonContent('../roles.json')
var roles = rolesJson.roles
var appConfigurationReader = 'App Configuration Data Reader'

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: configStore.userAssignedIdentityName
}
resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  location: location
  name: configStore.appConfigurationName
  sku: {
    name: 'standard'
  }
  properties: {}
}

resource managedIdentityCanReadConfigurationStore 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  name: guid(roles[appConfigurationReader], userAssignedIdentity.id)
  scope: appConfiguration
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[appConfigurationReader])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}


// Adding App Configuration entries
module appConfigurationSettings 'addAppConfiguration.bicep' = [ for entry in configStore.entries: {
  name: replace('AppConfigurationSettings-${entry.key}', ':', '-')
  params: {
    appConfigurationName: configStore.appConfigurationName
    userAssignedIdentityName: configStore.userAssignedIdentityName
    keyVaultName: configStore.keyVaultName
    value: entry.value
    keyName: entry.key
    label: configStore.label
    isSecret: entry.isSecret
  }
}]

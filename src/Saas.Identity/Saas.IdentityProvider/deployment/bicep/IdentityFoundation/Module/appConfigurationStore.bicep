@description('The location for all resources.')
param location string = resourceGroup().location

@description('User Assigned Identity Name.')
param userAssignedIdentityName string = 'userAssignedIdentityName'

@description('App Configuration Name.')
param appConfigurationName string

var rolesJson = loadJsonContent('roles.json')
var roles = rolesJson.roles
var appConfigurationReader = 'App Configuration Data Reader'

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource appConfigationStore 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  location: location
  name: appConfigurationName
  sku: {
    name: 'standard'
  }
  properties: {}
}

resource managedIdentityCanReadConfigurationStore 'Microsoft.Authorization/roleAssignments@2022-04-01' ={
  name: guid(roles[appConfigurationReader], userAssignedIdentity.id)
  scope: appConfigationStore
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[appConfigurationReader])
    principalId: userAssignedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}


@description('The name of the Azure App Configuration.')
param appConfigurationName string

@description('The name of the Azure KeyVault.')
param keyVaultName string

@description('URL for downstream admin service.')
param appSettingsAdminServiceBaseUrl string

@description('URL for downstream admin service.')
param azureB2CDomain string

@description('The B2C login endpoint in format of https://(Tenant Name).b2clogin.com.')
param azureB2CLoginEndpoint string

@description('Tenant Id found on your AD B2C dashboard.')
param azureB2CTenantId string

@description('The Client Id found on registered Permissions API app page.')
param azureB2CPermissionApiClientId string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('Azure App Configuration User Assigned Identity Name.')
param userAssignedIdentityName string

@description('The location for all resources.')
param location string = resourceGroup().location

var rolesJson = loadJsonContent('../roles.json')
var roles = rolesJson.roles
var appConfigurationReader = 'App Configuration Data Reader'

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource appConfiguration 'Microsoft.AppConfiguration/configurationStores@2022-05-01' = {
  location: location
  name: appConfigurationName
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
module appSettingsAdminServiceBaseUrlEntry 'addAppConfiguration.bicep' = {
  name: 'AppSettingsAdminServiceBaseUrl'
  params: {
    appConfigurationName: appConfiguration.name
    value: appSettingsAdminServiceBaseUrl
    keyName: 'AppSettings__AdminServiceBaseUrl'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

module azureB2CDomainEntry 'addAppConfiguration.bicep' = {
  name: 'azureB2CDomain'
  params: {
    appConfigurationName: appConfiguration.name
    value: azureB2CDomain
    keyName: 'azureB2CDomain'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

module azureB2CLoginEndpointEntry 'addAppConfiguration.bicep' = {
  name: 'azureB2CLoginEndpoint'
  params: {
    appConfigurationName: appConfiguration.name
    value: azureB2CLoginEndpoint
    keyName: 'azureB2CLoginEndpoint'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

module azureB2CTenantIdEntry 'addAppConfiguration.bicep' = {
  name: 'azureB2CTenantId'
  params: {
    appConfigurationName: appConfiguration.name
    value: azureB2CTenantId
    keyName: 'azureB2CTenantId'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

module azureB2CPermissionApiClientIdEntry 'addAppConfiguration.bicep' = {
  name: 'azureB2CPermissionApiClientId'
  params: {
    appConfigurationName: appConfiguration.name
    value: azureB2CPermissionApiClientId
    keyName: 'azureB2CPermissionApiClientId'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

module sqlAdministratorLoginEntry 'addAppConfiguration.bicep' = {
  name: 'sqlAdministratorLogin'
  params: {
    appConfigurationName: appConfiguration.name
    value: sqlAdministratorLogin
    keyName: 'sqlAdministratorLogin'
    isSecret: false
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: keyVaultName
  }
}

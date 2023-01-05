// Parameters
//////////////////////////////////////////////////
@description('URL for downstream admin service.')
param appSettingsAdminServiceBaseUrl string

@description('postfix')
param solutionPostfix string

@description('Solution prefix')
param solutionPrefix string

@description('solution name')
param solutionName string

@description('The name of the key vault')
param keyVaultName string

@description('URL for downstream admin service.')
param azureB2CDomain string

@description('The B2C login endpoint in format of https://(Tenant Name).b2clogin.com.')
param azureB2CLoginEndpoint string

@description('Tenant Id found on your AD B2C dashboard.')
param azureB2CTenantId string

@description('The Client Id found on registered Permissions API app page.')
param azureB2CPermissionApiClientId string

@description('Permission API Name')
param permissionsApiName string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string = 'https://ghcr.io'

@description('The tag of the container image to deploy to the permissions api app service.')
param permissionsApiContainerImageTag string = 'ghcr.io/azure/azure-saas/asdk-permissions:v1.1'

@description('The location for all resources.')
param location string = resourceGroup().location

// Variables
//////////////////////////////////////////////////
var appServicePlanName = 'plan-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var appConfigurationName = 'appconfig-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlDatabaseName = 'sqldb-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlServerName = 'sql-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'

var userAssignedIdentityName = 'user-assign-id-${solutionPrefix}-${solutionName}-${solutionPostfix}' 

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedIdentityName
  location: location
}

resource identityKeyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

module appConfigurationModule './identityAppConfig.bicep' = {
  name: 'appConfigurationDeployment'
  params: {
    appConfigurationName: appConfigurationName
    location: location
    appSettingsAdminServiceBaseUrl: appSettingsAdminServiceBaseUrl
    azureB2CDomain: azureB2CDomain
    azureB2CLoginEndpoint: azureB2CLoginEndpoint
    azureB2CTenantId: azureB2CTenantId
    azureB2CPermissionApiClientId: azureB2CPermissionApiClientId
    sqlAdministratorLogin: sqlAdministratorLogin
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultName: identityKeyVault.name
  }
}

// Module - App Service Plan
//////////////////////////////////////////////////
module appServicePlanModule './identityAppServicePlan.bicep' = {
  name: 'appServicePlanDeployment'
  params: {
    appServicePlanName: appServicePlanName
    location: location
  }
}

// Module - Permissions SQL Database
//////////////////////////////////////////////////
module permissionsSqlModule './permissionsSql.bicep' = {
  name: 'permissionsSqlDeployment'
  params: {
    location: location
    permissionsSqlDatabaseName: permissionsSqlDatabaseName
    permissionsSqlServerName: permissionsSqlServerName
    sqlAdministratorLogin: sqlAdministratorLogin
  }
}

module sqlConnectionStringEntry 'addAppConfiguration.bicep' = {
  name: 'sqlAdministratorLoginPassword'
  params: {
    appConfigurationName: appConfigurationName
    value: permissionsSqlModule.outputs.permissionsSqlDatabaseConnectionString
    keyName: 'sqlAdministratorLoginPassword'
    isSecret: true
    keyVaultName:keyVaultName
    appConfigUserAssignedIdentityName: userAssignedIdentity.name
  }
}

// Module - Permissions Api
//////////////////////////////////////////////////
module permissionsApiModule 'permissionsApi.bicep' = {
  name: 'permissionsApiDeployment'
  params: {
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: identityKeyVault.properties.vaultUri
    location: location
    permissionsApiName: permissionsApiName
    containerRegistryUrl: containerRegistryUrl
    permissionsApiContainerImageTag: permissionsApiContainerImageTag
    userAssignedIdentityName: userAssignedIdentity.name
    appConfigurationName: appConfigurationName
  }
}

// Module - Key Vault - Access Policy
//////////////////////////////////////////////////
module keyVaultAccessPolicyModule 'identityKeyVaultAccessPolicies.bicep' = {
  name: 'keyVaultAccessPolicyDeployment'
  params: {
    keyVaultName: keyVaultName
    userAssignedIdentityName: userAssignedIdentity.name
  }
}

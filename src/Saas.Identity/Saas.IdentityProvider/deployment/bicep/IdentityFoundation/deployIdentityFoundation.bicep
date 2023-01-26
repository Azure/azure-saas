@description('Version')
param version string

@description('The ip address of the dev machine')
param devMachineIp string

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
param permissionApiClientId string

@description('Permissions API Certificate Name')
param permissionCertificateName string

@description('Permissions API Instance')
param permissionInstance string

@description('Permission API Name')
param permissionsApiName string

@description('Permissions API Secret key')
param permissionApiKey string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('The location for all resources.')
param location string = resourceGroup().location

var appServicePlanName = 'plan-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var appConfigurationName = 'appconfig-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlDatabaseName = 'sqldb-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlServerName = 'sql-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var userAssignedIdentityName = 'user-assign-id-${solutionPrefix}-${solutionName}-${solutionPostfix}' 
var applicationInsightsName = 'appi-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var logAnalyticsWorkspaceName = 'log-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var automationAccountName = 'aa-${solutionPrefix}-${solutionName}-${solutionPostfix}'

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedIdentityName
  location: location
}
module secretGenerator './Module/createSecret.bicep' = {
  name: 'SecretsGenerator'
  params: {
    location: location
  }
}
module permissionsSqlModule './Module/permissionsSql.bicep' = {
  name: 'PermissionsSqlDeployment'
  params: {
    devMachineIp: devMachineIp
    location: location
    userAssignedIdentityName: userAssignedIdentity.name
    permissionsSqlDatabaseName: permissionsSqlDatabaseName
    permissionsSqlServerName: permissionsSqlServerName
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorLoginPassword: secretGenerator.outputs.secret
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

// Create object w/ array of objects containing the kayname and value to be stored in Azure App Configuration store.
var permissionApi = 'PermissionApi'

module appConfigurationModule './Module/appConfigurationStore.bicep' = {
  name: 'AppConfigurationDeployment'
  params: {
    appConfigurationName: appConfigurationName
    userAssignedIdentityName: userAssignedIdentity.name
    location: location
  }
}
module keyVaultAccessPolicyModule 'Module/keyVaultAccessRBAC.bicep' = {
  name: 'KeyVaultAccessPolicyDeployment'
  params: {
    keyVaultName: keyVault.name
    userAssignedIdentityName: userAssignedIdentity.name
  }
  dependsOn: [
    keyVault
  ]
}
module restApiKeyModule './Module/linkToExistingKeyVaultSecret.bicep' = {
  name: 'PermissionApiKeyDeployment'
  params: {
    label: version
    keyVaultName: keyVault.name
    appConfigurationName: appConfigurationName
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultKeyName: permissionApiKey
    keyName: '${permissionApi}:apiKey'
  }
  dependsOn: [
    keyVaultAccessPolicyModule
    keyVault
    appConfigurationModule
  ]
}

resource appConfigurationStore 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

module permissionsApiModule './Module/permissionsApi.bicep' = {
  name: 'PermissionsApiDeployment'
  params: {
    version: version
    appServicePlanName: appServicePlanName
    keyVaultUri: keyVault.properties.vaultUri
    location: location
    permissionsApiName: permissionsApiName
    userAssignedIdentityName: userAssignedIdentity.name
    appConfigurationName: appConfigurationStore.name
    applicationInsightsName: applicationInsightsName
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    automationAccountName: automationAccountName
  }
  dependsOn:  [
    appConfigurationModule
    keyVaultAccessPolicyModule
    keyVault
    restApiKeyModule
  ]
}

module configurationEntriesModule './Module/addConfigEntries.bicep' = {
  name: 'ConfigurationEntriesDeployment'
  params: {
    version: version
    appSettingsAdminServiceBaseUrl: appSettingsAdminServiceBaseUrl
    keyVaultName: keyVault.name
    azureB2CDomain: azureB2CDomain
    azureB2CLoginEndpoint: azureB2CLoginEndpoint
    azureB2CTenantId: azureB2CTenantId
    permissionApiClientId: permissionApiClientId
    permissionCertificateName: permissionCertificateName
    permissionInstance: permissionInstance
    sqlAdministratorLogin: sqlAdministratorLogin
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultUrl: keyVault.properties.vaultUri
    appConfigurationName: appConfigurationStore.name
    permissionsSqlDatabaseName: permissionsSqlDatabaseName
    permissionsSqlServerFQDN: permissionsSqlModule.outputs.permissionsSqlServerFQDN
    sqlAdministratorLoginPassword: secretGenerator.outputs.secret
  }
  dependsOn: [
    appConfigurationModule
    keyVaultAccessPolicyModule
    keyVault
    restApiKeyModule
    permissionsApiModule
  ]
}

output version string = version
output location string = location
output appConfigurationName string = appConfigurationName
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output appServicePlanName string = permissionsApiModule.outputs.appServicePlanName
output permissionsSqlServerName string = permissionsSqlModule.outputs.permissionsSqlServerName
output userAssignedIdentityName string = userAssignedIdentity.name
output userAssignedIdentityId string = userAssignedIdentity.id
output permissionsSqlServerFQDN string = permissionsSqlModule.outputs.permissionsSqlServerFQDN
output permissionsApiHostName string = permissionsApiModule.outputs.permissionsApiHostName
output applicationInsightsName string = applicationInsightsName
output logAnalyticsWorkspaceName string = logAnalyticsWorkspaceName
output automationAccountName string = automationAccountName

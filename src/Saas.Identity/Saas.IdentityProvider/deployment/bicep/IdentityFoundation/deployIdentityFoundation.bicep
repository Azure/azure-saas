@description('Version')
param version string

@description('Environment')
@allowed([
  'Development'
  'Staging'
  'Production'
])
param environment string

@description('The ip address of the dev machine')
param devMachineIp string


@description('postfix')
param solutionPostfix string

@description('Solution prefix')
param solutionPrefix string

@description('solution name')
param solutionName string

@description('The name of the key vault')
param keyVaultName string

@description('Permissions API Secret key')
param permissionApiKey string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('The location for all resources.')
param location string = resourceGroup().location

var appServicePlanOS = 'windows'
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

// Create object w/ array of objects containing the kayname and value to be stored in Azure App Configuration store.
var permissionsApiKeyName = 'PermissionsApi'
module restApiKeyModule './Module/linkToExistingKeyVaultSecret.bicep' = {
  name: 'PermissionApiKeyDeployment'
  params: {
    label: version
    keyVaultName: keyVault.name
    appConfigurationName: appConfigurationName
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultKeyName: permissionApiKey
    keyName: '${permissionsApiKeyName}:ApiKey'
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

module appPlanModule './Module/appPlan.bicep' = {
  name: 'PermissionsApiDeployment'
  params: {
    appServicePlanOS: appServicePlanOS
    appServicePlanName: appServicePlanName
    location: location
    userAssignedIdentityName: userAssignedIdentity.name
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
    keyVaultName: keyVault.name
    sqlAdministratorLogin: sqlAdministratorLogin
    userAssignedIdentityName: userAssignedIdentity.name
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
    appPlanModule
  ]
}

output version string = version
output location string = location
output environment string = environment
output appConfigurationName string = appConfigurationName
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output appServicePlanName string = appPlanModule.outputs.appServicePlanName
output permissionsSqlServerName string = permissionsSqlModule.outputs.permissionsSqlServerName
output userAssignedIdentityName string = userAssignedIdentity.name
output userAssignedIdentityId string = userAssignedIdentity.id
output permissionsSqlServerFQDN string = permissionsSqlModule.outputs.permissionsSqlServerFQDN
output applicationInsightsName string = applicationInsightsName
output logAnalyticsWorkspaceName string = logAnalyticsWorkspaceName
output automationAccountName string = automationAccountName

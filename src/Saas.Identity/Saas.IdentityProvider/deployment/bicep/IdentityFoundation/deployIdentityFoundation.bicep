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

@description('The git repo url.')
param gitRepoUrl string

@description('The git repo branch you want to deploy.')
param gitBranch string

@description('The location for all resources.')
param location string = resourceGroup().location

var appServicePlanName = 'plan-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var appConfigurationName = 'appconfig-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlDatabaseName = 'sqldb-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var permissionsSqlServerName = 'sql-permissions-${solutionPrefix}-${solutionName}-${solutionPostfix}'
var userAssignedIdentityName = 'user-assign-id-${solutionPrefix}-${solutionName}-${solutionPostfix}' 

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
var azureB2C = 'AzureB2C'
var permissionApi = 'PermissionApi'
var msGraph = 'MsGraph'
var sql = 'Sql'
var label = version

var permissionCertificates = [
  {
    SourceType: keyVault.name
    KeyVaultUrl: keyVault.properties.vaultUri
    KeyVaultCertificateName: permissionCertificateName
    }
]

var appConfigStore = {
  appConfigurationName: appConfigurationName
  keyVaultName: keyVault.name
  userAssignedIdentityName: userAssignedIdentity.name
  label: label
  entries: [
    {
      key: '${azureB2C}:AdminServiceBaseUrl'
      value: appSettingsAdminServiceBaseUrl
      isSecret: false
    }
    {
      key: '${azureB2C}:Domain'
      value: azureB2CDomain
      isSecret: false
    }
    {
      key: '${azureB2C}:LoginEndpoint'
      value: azureB2CLoginEndpoint
      isSecret: false
    }
    {
      key: '${azureB2C}:TenantId'
      value: azureB2CTenantId
      isSecret: false
    }
    {
      key: '${permissionApi}:ClientId'
      value: permissionApiClientId
      isSecret: false
    }
    {
      key: '${permissionApi}:TenantId'
      value: azureB2CTenantId
      isSecret: false
    }
    {
      key: '${permissionApi}:Domain'
      value: azureB2CDomain
      isSecret: false
    }
    {
      key: '${permissionApi}:Instance'
      value: permissionInstance
      isSecret: false
    }
    {
      key: '${permissionApi}:Audience'
      value: permissionApiClientId
      isSecret: false
    }
    {
      key: '${permissionApi}:CallbackPath'
      value: '/signin-oidc'
      isSecret: false
    }
    {
      key: '${permissionApi}:SignedOutCallbackPath'
      value: '/signout-oidc'
      isSecret: false
    }
    {
      key: '${permissionApi}:Certificates'
      value: replace('${permissionCertificates}', '\'','"') // replace single quotes with double quotes in the json string
      isSecret: false
    }
    {
      key: '${sql}:SQLAdministratorLoginName'
      value: sqlAdministratorLogin
      isSecret: false
    }
    {
      key: '${sql}:SQLAdministratorLoginPassword'
      value: secretGenerator.outputs.secret
      isSecret: true
    }
    {
      key: '${sql}:SQLConnectionString'
      value: 'Data Source=tcp:${permissionsSqlModule.outputs.permissionsSqlServerFQDN},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${permissionsSqlModule.outputs.permissionsSqlServerFQDN};Password=${secretGenerator.outputs.secret};'
      isSecret: true
    }
    {
      key: '${msGraph}:BaseUrl'
      value: 'https://graph.microsoft.com/v1.0'
      isSecret: false
    }
    {
      key: '${msGraph}:Scopes'
      value: 'https://graph.microsoft.com/.default'
      isSecret: false
    }
  ]
}

module appConfigurationModule './Module/appConfig.bicep' = {
  name: 'AppConfigurationDeployment'
  params: {
    configStore: appConfigStore
    location: location
  }
}

module permissionsApiModule 'Module/permissionsApi.bicep' = {
  name: 'PermissionsApiDeployment'
  params: {
    version: version
    appServicePlanName: appServicePlanName
    keyVaultUri: keyVault.properties.vaultUri
    location: location
    permissionsApiName: permissionsApiName
    userAssignedIdentityName: userAssignedIdentity.name
    appConfigurationName: appConfigurationModule.outputs.appConfigurationName
  }
}

module keyVaultAccessPolicyModule 'Module/keyVaultAccessPolicies.bicep' = {
  name: 'KeyVaultAccessPolicyDeployment'
  params: {
    keyVaultName: keyVaultName
    userAssignedIdentityName: userAssignedIdentity.name
  }
}

module restApiKeyModule './Module/linkToExistingKeyVaultSecret.bicep' = {
  name: 'PermissionApiKeyDeployment'
  params: {
    label: version
    keyVaultName: keyVaultName
    appConfigurationName: appConfigStore.appConfigurationName
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultKeyName: permissionApiKey
    keyName: '${permissionApi}:apiKey'
  }
}

output version string = version
output location string = location
output appConfigurationName string = appConfigStore.appConfigurationName
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output appServicePlanName string = permissionsApiModule.outputs.appServicePlanName
output permissionsSqlServerName string = permissionsSqlModule.outputs.permissionsSqlServerName
output userAssignedIdentityName string = userAssignedIdentity.name
output userAssignedIdentityId string = userAssignedIdentity.id
output permissionsSqlServerFQDN string = permissionsSqlModule.outputs.permissionsSqlServerFQDN
output permissionsApiHostName string = permissionsApiModule.outputs.permissionsApiHostName

// Parameters
//////////////////////////////////////////////////
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

// Module - App Service Plan
//////////////////////////////////////////////////
module appServicePlanModule './identityAppServicePlan.bicep' = {
  name: 'appServicePlanDeployment'
  params: {
    appServicePlanName: appServicePlanName
    location: location
  }
}

module secretGenerator 'createSecret.bicep' = {
  name: 'secrets'
  params: {
    location: location
  }
}

// Module - Permissions SQL Database
//////////////////////////////////////////////////
module permissionsSqlModule './permissionsSql.bicep' = {
  name: 'permissionsSqlDeployment'
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

resource identityKeyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

// Create object w/ array of objects containing the kayname and value to be stored in Azure App Configuration store.
var azureB2C = 'AzureB2C'
var permissionApi = 'PermissionApi'
var sql = 'Sql'
var label = version

var permissionCertificates = [
  {
    SourceType: identityKeyVault.name
    KeyVaultUrl: identityKeyVault.properties.vaultUri
    KeyVaultCertificateName: permissionCertificateName
    }
]

var appConfigStore = {
  appConfigurationName: appConfigurationName
  keyVaultName: identityKeyVault.name
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
  ]
}

module appConfigurationModule './identityAppConfig.bicep' = {
  name: 'appConfigurationDeployment'
  params: {
    configStore: appConfigStore
    location: location
  }
}

// Module - Permissions Api
//////////////////////////////////////////////////
module permissionsApiModule 'permissionsApi.bicep' = {
  name: 'permissionsApiDeployment'
  params: {
    version: version
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

module restApiKeyModule './linkToExistingKeyVaultSecret.bicep' = {
  name: 'permissionApiKeyDeployment'
  params: {
    label: version
    keyVaultName: keyVaultName
    appConfigurationName: appConfigStore.appConfigurationName
    userAssignedIdentityName: userAssignedIdentity.name
    keyVaultKeyName: permissionApiKey
    keyName: '${permissionApi}:apiKey'
  }
}

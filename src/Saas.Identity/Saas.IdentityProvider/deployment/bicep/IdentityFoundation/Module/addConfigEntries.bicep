@description('Version')
param version string

@description('URL for downstream admin service.')
param appSettingsAdminServiceBaseUrl string

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

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('User Identity Name')
param userAssignedIdentityName string

@description('Key Vault Url')
param keyVaultUrl string

@description('App Configuration Name')
param appConfigurationName string

@description('Permissions SQL Database Name')
param permissionsSqlDatabaseName string

@description('Permissions SQL FQDN')
param permissionsSqlServerFQDN string

@description('SQL Login Password')
@secure()
param sqlAdministratorLoginPassword string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

// Create object with array of objects containing the kayname and value to be stored in Azure App Configuration store.

var azureB2C = 'AzureB2C'
var permissionApi = 'PermissionApi'
var msGraph = 'MsGraph'
var sql = 'Sql'

var permissionCertificates = [
  {
    SourceType: keyVaultName
    KeyVaultUrl: keyVaultUrl
    KeyVaultCertificateName: permissionCertificateName
  }
]

var appConfigStore = {
  appConfigurationName: appConfigurationName
  keyVaultName: keyVaultName
  userAssignedIdentityName: userAssignedIdentity.name
  label: version
  entries: [
    {
      key: '${azureB2C}:AdminServiceBaseUrl'
      value: appSettingsAdminServiceBaseUrl
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${azureB2C}:Domain'
      value: azureB2CDomain
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${azureB2C}:LoginEndpoint'
      value: azureB2CLoginEndpoint
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${azureB2C}:TenantId'
      value: azureB2CTenantId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:ClientId'
      value: permissionApiClientId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:TenantId'
      value: azureB2CTenantId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:Domain'
      value: azureB2CDomain
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:Instance'
      value: permissionInstance
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:Audience'
      value: permissionApiClientId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:CallbackPath'
      value: '/signin-oidc'
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:SignedOutCallbackPath'
      value: '/signout-oidc'
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${permissionApi}:Certificates'
      value: replace('${permissionCertificates}', '\'','"') // replace single quotes with double quotes in the json string
      isSecret: false
      contentType: 'application/json'
    }
    {
      key: '${sql}:SQLAdministratorLoginName'
      value: sqlAdministratorLogin
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${sql}:SQLAdministratorLoginPassword'
      value: sqlAdministratorLoginPassword
      isSecret: true
      contentType: 'text/plain'
    }
    {
      key: '${sql}:SQLConnectionString'
      value: 'Data Source=tcp:${permissionsSqlServerFQDN},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${permissionsSqlServerFQDN};Password=${sqlAdministratorLoginPassword};'
      isSecret: true
      contentType: 'text/plain'
    }
    {
      key: '${msGraph}:BaseUrl'
      value: 'https://graph.microsoft.com/v1.0'
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${msGraph}:Scopes'
      value: 'https://graph.microsoft.com/.default'
      isSecret: false
      contentType: 'text/plain'
    }
  ]
}

// Adding App Configuration entries
module appConfigurationSettings 'addConfigEntry.bicep' = [ for entry in appConfigStore.entries: {
  name: replace('AppConfigurationSettings-${entry.key}', ':', '-')
  params: {
    appConfigurationName: appConfigStore.appConfigurationName
    userAssignedIdentityName: appConfigStore.userAssignedIdentityName
    keyVaultName: keyVaultName
    value: entry.value
    contentType: entry.contentType
    keyName: entry.key
    label: appConfigStore.label
    isSecret: entry.isSecret
  }
}]

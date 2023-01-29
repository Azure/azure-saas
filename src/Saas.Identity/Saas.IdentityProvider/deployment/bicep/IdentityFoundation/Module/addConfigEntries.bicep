@description('Version')
param version string

@description('The name of the key vault')
param keyVaultName string

// @description('URL for downstream admin service.')
// param azureB2CInstance string

// @description('URL for downstream admin service.')
// param azureB2CClientId string

// @description('URL for downstream admin service.')
// param azureB2CDomain string

// @description('The B2C login endpoint in format of https://(Tenant Name).b2clogin.com.')
// param azureB2CLoginEndpoint string

// @description('Tenant Id found on your AD B2C dashboard.')
// param azureB2CTenantId string

// @description('The Azure B2C Signed Out Call Back Path.')
// param signedOutCallBackPath string

// @description('The Azure B2C Sign up/in Policy Id.')
// param signUpSignInPolicyId string

// @description('The Azure B2C Permissions API base Url.')
// param permissionsBaseUrl string

// @description('The Client Id found on registered Permissions API app page.')
// param permissionApiClientId string

// @description('Permissions API Certificate Name')
// param permissionCertificateName string

// @description('Permissions API Instance')
// param permissionInstance string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('User Identity Name')
param userAssignedIdentityName string

// @description('Key Vault Url')
// param keyVaultUrl string

@description('App Configuration Name')
param appConfigurationName string

@description('Permissions SQL Database Name')
param permissionsSqlDatabaseName string

@description('Permissions SQL FQDN')
param permissionsSqlServerFQDN string

@description('SQL Login Password')
@secure()
param sqlAdministratorLoginPassword string

// @description('PermissionsAPI key name')
// param permissionsApiKeyName string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

// Create object with array of objects containing the kayname and value to be stored in Azure App Configuration store.

var msGraphKeyName = 'MsGraph'
var sqlKeyName = 'Sql'

var appConfigStore = {
  appConfigurationName: appConfigurationName
  keyVaultName: keyVaultName
  userAssignedIdentityName: userAssignedIdentity.name
  label: version
  entries: [
    {
      key: '${sqlKeyName}:SQLAdministratorLoginName'
      value: sqlAdministratorLogin
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${sqlKeyName}:SQLAdministratorLoginPassword'
      value: sqlAdministratorLoginPassword
      isSecret: true
      contentType: 'text/plain'
    }
    {
      key: '${sqlKeyName}:SQLConnectionString'
      value: 'Data Source=tcp:${permissionsSqlServerFQDN},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${permissionsSqlServerFQDN};Password=${sqlAdministratorLoginPassword};'
      isSecret: true
      contentType: 'text/plain'
    }
    {
      key: '${msGraphKeyName}:BaseUrl'
      value: 'https://graph.microsoft.com/v1.0'
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${msGraphKeyName}:Scopes'
      value: 'https://graph.microsoft.com/.default'
      isSecret: false
      contentType: 'text/plain'
    }
  ]
}

// Adding App Configuration entries
module appConfigurationSettings './../../../../../../Saas.Lib/Saas.Bicep.Module/addConfigEntry.bicep' = [ for entry in appConfigStore.entries: {
  name: replace('Entry-${entry.key}', ':', '-')
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

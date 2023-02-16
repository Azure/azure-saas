@description('Version')
param version string

@description('The name of the key vault')
param keyVaultName string

@description('Select an admin account name used for resource creation.')
param sqlAdministratorLogin string

@description('User Identity Name')
param userAssignedIdentityName string

@description('App Configuration Name')
param appConfigurationName string

@description('Azure SQL Server FQDN')
param sqlServerFQDN string

@description('Permissions SQL Database Name')
param permissionsSqlDatabaseName string

@description('Tenant SQL Database Name')
param tenantSqlDatabaseName string

@description('SQL Login Password')
@secure()
param sqlAdministratorLoginPassword string

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
      key: '${sqlKeyName}:PermissionsSQLConnectionString'
      value: 'Data Source=tcp:${sqlServerFQDN},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${sqlServerFQDN};Password=${sqlAdministratorLoginPassword};'
      isSecret: true
      contentType: 'text/plain'
    }
    {
      key: '${sqlKeyName}:TenantSQLConnectionString'
      value: 'Data Source=tcp:${sqlServerFQDN},1433;Initial Catalog=${tenantSqlDatabaseName};User Id=${sqlAdministratorLogin}@${sqlServerFQDN};Password=${sqlAdministratorLoginPassword};'
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

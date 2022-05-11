// Parameters
//////////////////////////////////////////////////
@description('The location for all resources.')
param location string

@description('The Permissions SQL Server name.')
param permissionsSqlServerName string

@description('The Permissions SQL Database name.')
param permissionsSqlDatabaseName string

@description('The SQL Server administrator login.')
param sqlAdministratorLogin string

@description('The SQL Server administrator login password.')
@secure()
param sqlAdministratorLoginPassword string

// Resource - Permissions SQL Server
//////////////////////////////////////////////////
resource permissionsSqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: permissionsSqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
  }
}

// Resource - Permissions SQL Database
//////////////////////////////////////////////////
resource permissionsSqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: permissionsSqlServer
  name: permissionsSqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

// Outputs
//////////////////////////////////////////////////
output permissionsSqlServerFQDN string = permissionsSqlServer.properties.fullyQualifiedDomainName
output permissionsSqlDatabaseConnectionString string = 'Data Source=tcp:${permissionsSqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${permissionsSqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdministratorLoginPassword};'

// Parameters
//////////////////////////////////////////////////
@description('The location for all resources.')
param location string

@description('The SQL Server administrator login.')
param sqlAdministratorLogin string

@description('The SQL Server administrator login password.')
@secure()
param sqlAdministratorLoginPassword string

@description('The Admin SQL Server name.')
param adminSqlServerName string

@description('The Admin SQL Database name.')
param adminSqlDatabaseName string

// Resource - Admin SQL Server
//////////////////////////////////////////////////
resource adminSqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: adminSqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
  }
}

// Resource - Admin SQL Database
//////////////////////////////////////////////////
resource adminSqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: adminSqlServer
  name: adminSqlDatabaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

// Outputs
//////////////////////////////////////////////////
output adminSqlServerFQDN string = adminSqlServer.properties.fullyQualifiedDomainName
output adminSqlDatabaseConnectionString string = 'Data Source=tcp:${adminSqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${adminSqlDatabaseName};User Id=${sqlAdministratorLogin}@${adminSqlServer.properties.fullyQualifiedDomainName};Password=${sqlAdministratorLoginPassword};'

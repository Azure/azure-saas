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

// @description('The SQL Server administrator login password.')
// @secure()
// param sqlAdministratorLoginPassword string

module secretGenerator 'createSecret.bicep' = {
  name: 'secrets'
  params: {
    location: location
  }
}

// Resource - Permissions SQL Server
//////////////////////////////////////////////////
resource permissionsSqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: permissionsSqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: secretGenerator.outputs.secret
    version: '12.0'
  }
}

// Allow access to azure services checkbox
resource allowAzureAccessFirewallRule 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAllWindowsAzureIps'
  parent: permissionsSqlServer
  properties: {
    // Using 0.0.0.0 to specify all internal azure ips as found here: https://docs.microsoft.com/en-us/azure/templates/microsoft.sql/servers/firewallrules?tabs=bicep#serverfirewallruleproperties
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Resource - Permissions SQL Database
//////////////////////////////////////////////////
resource permissionsSqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
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
output permissionsSqlDatabaseConnectionString string = 'Data Source=tcp:${permissionsSqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${permissionsSqlDatabaseName};User Id=${sqlAdministratorLogin}@${permissionsSqlServer.properties.fullyQualifiedDomainName};Password=${secretGenerator.outputs.secret};'

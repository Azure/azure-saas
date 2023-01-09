// Parameters
//////////////////////////////////////////////////
@description('The ip address of the dev machine')
param devMachineIp string

@description('The location for all resources.')
param location string

@description('The Permissions SQL Server name.')
param permissionsSqlServerName string

@description('The Permissions SQL Database name.')
param permissionsSqlDatabaseName string

@description('The SQL Server administrator login.')
param sqlAdministratorLogin string

@description('The User Assigned Identity name.')
param userAssignedIdentityName string

@description('The SQL Server administrator login password.')
@secure()
param sqlAdministratorLoginPassword string

// Resource - Permissions SQL Server
//////////////////////////////////////////////////
resource permissionsSqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: permissionsSqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    version: '12.0'
  }
}

// Allow all internal azure ips to access the sql server firewall
resource allowAzureAccessFirewallRuleAzureInternal 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowOnlyAllWindowsAzureIps'
  parent: permissionsSqlServer
  properties: {
    // Using 0.0.0.0 to specify all internal azure ips as found here: https://docs.microsoft.com/en-us/azure/templates/microsoft.sql/servers/firewallrules?tabs=bicep#serverfirewallruleproperties
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Allow all internal azure ips to access the sql server firewall
resource allowAzureAccessFirewallRuleDevMachine 'Microsoft.Sql/servers/firewallRules@2022-05-01-preview' = {
  name: 'AllowAccessToDevMachineIpAddress'
  parent: permissionsSqlServer
  properties: {
    // Using 0.0.0.0 to specify all internal azure ips as found here: https://docs.microsoft.com/en-us/azure/templates/microsoft.sql/servers/firewallrules?tabs=bicep#serverfirewallruleproperties
    startIpAddress: devMachineIp
    endIpAddress: devMachineIp
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

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedIdentityName
  location: location
}

// Outputs
//////////////////////////////////////////////////
output permissionsSqlServerFQDN string = permissionsSqlServer.properties.fullyQualifiedDomainName

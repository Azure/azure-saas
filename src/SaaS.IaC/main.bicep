// Parameters
//////////////////////////////////////////////////
@description('Scopes to authorize user for the admin service.')
param adminApiScopes string

@description('The value of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretValue string

@description('The value of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretValue string

@description('The value of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretValue string

@description('The value of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretValue string

@description('The value of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretValue string

@description('The value of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretValue string

@description('The object ID of the logged in Azure Active Directory User.')
param azureAdUserID string

@description('The value of the Permissions Api Certificate Key Vault Secret.')
param permissionsApiCertificateSecretValue string

@description('The value of the Permissions Api SSL Thumbprint Key Vault Secret.')
param permissionsApiSslThumbprintSecretValue string

@description('The location for all resources.')
param location string = resourceGroup().location

@description('The SaaS Provider name.')
param saasProviderName string

@description('The deployment environment (e.g. prod, dev, test).')
@allowed([
  'prod'
  'staging'
  'dev'
  'test'
])
param saasEnvironment string = 'dev'

@description('The instance number of the deployment.')
@minLength(1)
@maxLength(3)
param saasInstanceNumber string

@description('The SQL Server administrator login.')
param sqlAdministratorLogin string

@description('The SQL Server administrator login password.')
@secure()
param sqlAdministratorLoginPassword string

// Variables
//////////////////////////////////////////////////
var adminApiName = replace('api-admin-${saasProviderName}-${saasEnvironment}', '-', '')
var adminSqlDatabaseName = 'sqldb-admin-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var adminSqlServerName = 'sql-admin-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var applicationAppServiceName = replace('app-application-${saasProviderName}-${saasEnvironment}', '-', '')
var appServicePlanName = 'plan-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var keyVaultName = 'kv-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var logicAppName = 'logic-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var permissionsApiName = replace('api-permissions-${saasProviderName}-${saasEnvironment}', '-', '')
var permissionsSqlDatabaseName = 'sqldb-permissions-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var permissionsSqlServerName = 'sql-permissions-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var signupAdminAppServiceName = replace('app-signup-${saasProviderName}-${saasEnvironment}', '-', '')

// Module - App Service Plan
//////////////////////////////////////////////////
module appServicePlanModule './appServicePlan.bicep' = {
  name: 'appServicePlanDeployment'
  params: {
    appServicePlanName: appServicePlanName
    location: location
  }
}

// Module - Logic App
//////////////////////////////////////////////////
module logicAppModule 'notifications.bicep' = {
  name: 'logicAppDeployment'
  params: {
    location: location
    logicAppName: logicAppName
  }
}

// Module - Permissions SQL Database
//////////////////////////////////////////////////
module permissionsSqlModule './permissionsSql.Bicep' = {
  name: 'permissionsSqlDeployment'
  params: {
    location: location
    permissionsSqlDatabaseName: permissionsSqlDatabaseName
    permissionsSqlServerName: permissionsSqlServerName
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorLoginPassword: sqlAdministratorLoginPassword
  }
}

// Module - Admin SQL Database
//////////////////////////////////////////////////
module adminSqlModule './adminSql.Bicep' = {
  name: 'adminSqlDeployment'
  params: {
    adminSqlDatabaseName: adminSqlDatabaseName
    adminSqlServerName: adminSqlServerName
    location: location
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorLoginPassword: sqlAdministratorLoginPassword
  }
}

// Module - Key Vault
//////////////////////////////////////////////////
module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    adminSqlConnectionStringSecretValue: adminSqlModule.outputs.adminSqlDatabaseConnectionString
    azureAdB2cAdminApiClientIdSecretValue: azureAdB2cAdminApiClientIdSecretValue
    azureAdB2cDomainSecretValue: azureAdB2cDomainSecretValue
    azureAdB2cInstanceSecretValue: azureAdB2cInstanceSecretValue
    azureAdB2cSignupAdminClientIdSecretValue: azureAdB2cSignupAdminClientIdSecretValue
    azureAdB2cSignupAdminClientSecretSecretValue: azureAdB2cSignupAdminClientSecretSecretValue
    azureAdB2cTenantIdSecretValue: azureAdB2cTenantIdSecretValue
    keyVaultName: keyVaultName
    location: location
    permissionsApiCertificateSecretValue: permissionsApiCertificateSecretValue
    permissionsApiSslThumbprintSecretValue: permissionsApiSslThumbprintSecretValue
    permissionsSqlConnectionStringSecretValue: permissionsSqlModule.outputs.permissionsSqlDatabaseConnectionString
  }
}

// Module - Permissions Api
//////////////////////////////////////////////////
module permissionsApiModule './permissionsApi.bicep' = {
  name: 'permissionsApiDeployment'
  params: {
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    permissionsApiName: permissionsApiName
  }
}

// Module - Admin Api
//////////////////////////////////////////////////
module adminApiModule './adminApi.bicep' = {
  name: 'adminApiDeployment'
  params: {
    adminApiName: adminApiName
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    permissionsApiHostName: permissionsApiModule.outputs.permissionsApiHostName
  }
}

// Module - Signup Administration App Service
//////////////////////////////////////////////////
module signupAdminAppServiceModule 'signupAdminWeb.bicep' = {
  name: 'signupAdminAppServiceDeployment'
  params: {
    adminApiHostName: adminApiModule.outputs.adminApiHostName
    adminApiScopes: adminApiScopes
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    signupAdminAppServiceName: signupAdminAppServiceName
  }
}

// Module - Application App Service
//////////////////////////////////////////////////
module applicationAppServiceModule 'applicationWeb.bicep' = {
  name: 'applicationAppServiceDeployment'
  params: {
    applicationAppServiceName: applicationAppServiceName
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    location: location
  }
}

// Module - Key Vault - Access Policy
//////////////////////////////////////////////////
module keyVaultAccessPolicyModule 'keyVaultAccessPolicies.bicep' = {
  name: 'keyVaultAccessPolicyDeployment'
  params: {
    adminApiPrincipalId: adminApiModule.outputs.systemAssignedManagedIdentityPrincipalId
    azureAdUserID: azureAdUserID
    keyVaultName: keyVaultName
    permissionApiPrincipalId: permissionsApiModule.outputs.systemAssignedManagedIdentityPrincipalId
    signupAdminAppServicePrincipalId: signupAdminAppServiceModule.outputs.systemAssignedManagedIdentityPrincipalId
  }
}

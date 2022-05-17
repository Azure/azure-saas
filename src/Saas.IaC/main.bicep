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

@description('Deploy the AdminService module. Defaults to true.')
param deployAdminServiceModule bool = true

@description('Deploy the ApplicationWeb module. Defaults to true.')
param deployApplicationWebModule bool = true

@description('Deploy the PermissionsService module. Defaults to true.')
param deployPermissionsServiceModule bool = true

@description('Deploy the SignupWeb module. Defaults to true.')
param deploySignupAdminWebModule bool = true

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

var modulesToDeploy = {
  adminService: deployAdminServiceModule
  applicationWeb: deployApplicationWebModule
  permissionsService: deployPermissionsServiceModule
  signupAdminWeb: deploySignupAdminWebModule
}

var messageToUpdate = 'UPDATE THIS AFTER DEPLOYMENT'

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
module permissionsSqlModule './permissionsSql.bicep' = if (modulesToDeploy.permissionsService) {
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
module adminSqlModule './adminSql.bicep' = if (modulesToDeploy.adminService) {
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
    adminSqlConnectionStringSecretValue: (modulesToDeploy.adminService) ? adminSqlModule.outputs.adminSqlDatabaseConnectionString : ''
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
    permissionsSqlConnectionStringSecretValue: (modulesToDeploy.permissionsService) ? permissionsSqlModule.outputs.permissionsSqlDatabaseConnectionString : ''
  }
}

// Module - Permissions Api
//////////////////////////////////////////////////
module permissionsApiModule './permissionsApi.bicep' = if (modulesToDeploy.permissionsService) {
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
module adminApiModule './adminApi.bicep' = if (modulesToDeploy.adminService) {
  name: 'adminApiDeployment'
  params: {
    adminApiName: adminApiName
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    permissionsApiHostName: (modulesToDeploy.permissionsService) ? permissionsApiModule.outputs.permissionsApiHostName : messageToUpdate
  }
}

// Module - Signup Administration App Service
//////////////////////////////////////////////////
module signupAdminAppServiceModule 'signupAdminWeb.bicep' = if (modulesToDeploy.signupAdminWeb) {
  name: 'signupAdminAppServiceDeployment'
  params: {
    adminApiHostName: (modulesToDeploy.adminService) ? adminApiModule.outputs.adminApiHostName : messageToUpdate
    adminApiScopes: adminApiScopes
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    signupAdminAppServiceName: signupAdminAppServiceName
  }
}

// Module - Application App Service
//////////////////////////////////////////////////
module applicationAppServiceModule 'applicationWeb.bicep' = if (modulesToDeploy.applicationWeb) {
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
    modulesToDeploy: modulesToDeploy
    permissionApiPrincipalId: permissionsApiModule.outputs.systemAssignedManagedIdentityPrincipalId
    signupAdminAppServicePrincipalId: signupAdminAppServiceModule.outputs.systemAssignedManagedIdentityPrincipalId
  }
}

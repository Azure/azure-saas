// Parameters
//////////////////////////////////////////////////
@description('Scopes to authorize user for the admin service.')
param adminApiScopes string

@description('The base url for the app registration that the scopes belong to.')
param adminApiScopeBaseUrl string

@description('The tag of the container image to deploy to the Admin api app service.')
param adminApiContainerImageTag string = 'ghcr.io/azure/azure-saas/asdk-admin:v1.1'

@description('The tag of the container image to deploy to the SaaS Application api app service.')
param applicationContainerImageTag string = 'ghcr.io/azure/azure-saas/asdk-web:v1.1'

@description('The value of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretValue string

@description('The value of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretValue string

@description('The value of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretValue string

@description('The value of the Azure AD B2C SaaS App Client Id Key Vault Secret.')
param azureAdB2cSaasAppClientIdSecretValue string

@description('The value of the Azure AD B2C SaaS App Client Secret Key Vault Secret.')
param azureAdB2cSaasAppClientSecretSecretValue string

@description('The value of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretValue string

@description('The value of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretValue string

@description('The value of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretValue string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string = 'https://ghcr.io'

@description('Deploy the AdminService module. Defaults to true.')
param deployAdminServiceModule bool = true

@description('Deploy the ApplicationWeb module. Defaults to true.')
param deployApplicationWebModule bool = true

@description('Deploy the PermissionsService module. Defaults to true.')
param deployPermissionsServiceModule bool = true

@description('Deploy the SignupWeb module. Defaults to true.')
param deploySignupAdminWebModule bool = true

@description('The location for all resources.')
param location string = resourceGroup().location

@description('The Host Name of the Permissions Api to point the Admin Api to.')
param permissionsApiHostName string

@description('The API Key used to authenticate with the Permissions Api.')
@secure()
param permissionsApiApiKeySecretValue string

@description('Scopes to authorize SaaS App user for the admin service.')
param saasAppApiScopes string

@description('The tag of the container image to deploy to the SignupAdmin app service.')
param signupAdminContainerImageTag string = 'ghcr.io/azure/azure-saas/asdk-signup:v1.1'

@description('The SaaS Provider name.')
param saasProviderName string

@description('The deployment environment (e.g. prd, dev, tst).')
@allowed([
  'prd'
  'stg'
  'dev'
  'tst'
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
    azureAdB2cSaasAppClientIdSecretValue: azureAdB2cSaasAppClientIdSecretValue
    azureAdB2cSaasAppClientSecretSecretValue: azureAdB2cSaasAppClientSecretSecretValue
    azureAdB2cSignupAdminClientIdSecretValue: azureAdB2cSignupAdminClientIdSecretValue
    azureAdB2cSignupAdminClientSecretSecretValue: azureAdB2cSignupAdminClientSecretSecretValue
    azureAdB2cTenantIdSecretValue: azureAdB2cTenantIdSecretValue
    keyVaultName: keyVaultName
    location: location
    permissionsApiApiKeySecretValue: permissionsApiApiKeySecretValue
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
    permissionsApiHostName: permissionsApiHostName
    adminApiContainerImageTag: adminApiContainerImageTag
    containerRegistryUrl: containerRegistryUrl
  }
}

// Module - Signup Administration App Service
//////////////////////////////////////////////////
module signupAdminAppServiceModule 'signupAdminWeb.bicep' = if (modulesToDeploy.signupAdminWeb) {
  name: 'signupAdminAppServiceDeployment'
  params: {
    adminApiHostName: (modulesToDeploy.adminService) ? adminApiModule.outputs.adminApiHostName : messageToUpdate
    adminApiScopes: adminApiScopes
    adminApiScopeBaseUrl: adminApiScopeBaseUrl
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    signupAdminAppServiceName: signupAdminAppServiceName
    signupAdminApiContainerImageTag: signupAdminContainerImageTag
    containerRegistryUrl: containerRegistryUrl
  }
}

// Module - Application App Service
//////////////////////////////////////////////////
module applicationAppServiceModule 'applicationWeb.bicep' = if (modulesToDeploy.applicationWeb) {
  name: 'applicationAppServiceDeployment'
  params: {
    adminApiHostName: (modulesToDeploy.adminService) ? adminApiModule.outputs.adminApiHostName : messageToUpdate
    saasAppApiScopes: saasAppApiScopes
    adminApiScopeBaseUrl: adminApiScopeBaseUrl
    applicationAppServiceName: applicationAppServiceName
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: keyVaultModule.outputs.keyVaultUri
    location: location
    applicationApiContainerImageTag: applicationContainerImageTag
    containerRegistryUrl: containerRegistryUrl
  }
}

// Module - Key Vault - Access Policy
//////////////////////////////////////////////////
module keyVaultAccessPolicyModule 'keyVaultAccessPolicies.bicep' = {
  name: 'keyVaultAccessPolicyDeployment'
  params: {
    adminApiPrincipalId: adminApiModule.outputs.systemAssignedManagedIdentityPrincipalId
    keyVaultName: keyVaultName
    modulesToDeploy: modulesToDeploy
    applicationAppServicePrincipalId: applicationAppServiceModule.outputs.systemAssignedManagedIdentityPrincipalId
    signupAdminAppServicePrincipalId: signupAdminAppServiceModule.outputs.systemAssignedManagedIdentityPrincipalId
  }
}

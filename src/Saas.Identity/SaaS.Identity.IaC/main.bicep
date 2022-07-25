// Parameters
//////////////////////////////////////////////////


@description('The value of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretValue string

@description('The value of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretValue string

@description('The value of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretValue string

@description('The value of the Azure AD B2C Permissions Api Client Id Key Vault Secret.')
param azureAdB2cPermissionsApiClientIdSecretValue string 

@description('The value of the Azure AD B2C Permissions Api Client Secret Key Vault Secret.')
param azureAdB2cPermissionsApiClientSecretSecretValue string 

@description('The value of the Permissions Api Api Key Key Vault Secret.')
param permissionsApiApiKeySecretValue string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string = 'https://ghcr.io'

@description('The tag of the container image to deploy to the permissions api app service.')
param permissionsApiContainerImageTag string = 'ghcr.io/azure/azure-saas/asdk-permissions:v1.1'

@description('The location for all resources.')
param location string = resourceGroup().location

@description('The SaaS Provider name.')
@maxLength(8)
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
var appServicePlanName = 'plan-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var identityKeyVaultName = 'kv-identity-${saasProviderName}-${saasEnvironment}'
var permissionsApiName = replace('api-permissions-${saasProviderName}-${saasEnvironment}', '-', '')
var permissionsSqlDatabaseName = 'sqldb-permissions-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'
var permissionsSqlServerName = 'sql-permissions-${saasProviderName}-${saasEnvironment}-${saasInstanceNumber}'


// Module - App Service Plan
//////////////////////////////////////////////////
module appServicePlanModule './identityAppServicePlan.bicep' = {
  name: 'appServicePlanDeployment'
  params: {
    appServicePlanName: appServicePlanName
    location: location
  }
}

// Module - Permissions SQL Database
//////////////////////////////////////////////////
module permissionsSqlModule './permissionsSql.bicep' = {
  name: 'permissionsSqlDeployment'
  params: {
    location: location
    permissionsSqlDatabaseName: permissionsSqlDatabaseName
    permissionsSqlServerName: permissionsSqlServerName
    sqlAdministratorLogin: sqlAdministratorLogin
    sqlAdministratorLoginPassword: sqlAdministratorLoginPassword
  }
}

// Module - Key Vault
//////////////////////////////////////////////////
module identityKeyVaultModule './identityKeyVault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    keyVaultName: identityKeyVaultName
    location: location
    azureAdB2cDomainSecretValue: azureAdB2cDomainSecretValue
    azureAdB2cInstanceSecretValue: azureAdB2cInstanceSecretValue
    azureAdB2cTenantIdSecretValue: azureAdB2cTenantIdSecretValue
    azureAdB2cPermissionsApiClientIdSecretValue: azureAdB2cPermissionsApiClientIdSecretValue
    azureAdB2cPermissionsApiClientSecretSecretValue: azureAdB2cPermissionsApiClientSecretSecretValue
    permissionsApiApiKeySecretValue: permissionsApiApiKeySecretValue
    permissionsSqlConnectionStringSecretValue: permissionsSqlModule.outputs.permissionsSqlDatabaseConnectionString
  }
}

// Module - Permissions Api
//////////////////////////////////////////////////
module permissionsApiModule 'permissionsApi.bicep' = {
  name: 'permissionsApiDeployment'
  params: {
    appServicePlanId: appServicePlanModule.outputs.appServicePlanId
    keyVaultUri: identityKeyVaultModule.outputs.keyVaultUri
    location: location
    permissionsApiName: permissionsApiName
    containerRegistryUrl: containerRegistryUrl
    permissionsApiContainerImageTag: permissionsApiContainerImageTag
  }
}

// Module - Key Vault - Access Policy
//////////////////////////////////////////////////
module keyVaultAccessPolicyModule 'identityKeyVaultAccessPolicies.bicep' = {
  name: 'keyVaultAccessPolicyDeployment'
  params: {
    keyVaultName: identityKeyVaultName
    permissionApiPrincipalId: permissionsApiModule.outputs.systemAssignedManagedIdentityPrincipalId
  }
}

// Parameters
//////////////////////////////////////////////////
@description('The name of the Admin SQL Connection String Key Vault Secret.')
param adminSqlConnectionStringSecretName string = 'ConnectionStrings--TenantsContext'

@description('The value of the Admin SQL Connection String Key Vault Secret.')
param adminSqlConnectionStringSecretValue string

@description('The name of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretName string = 'AzureAdB2C--AdminApiClientId'

@description('The value of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretValue string

@description('The name of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretName string = 'AzureAdB2C--Domain'

@description('The value of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretValue string

@description('The name of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretName string = 'AzureAdB2C--Instance'

@description('The value of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretValue string

@description('The name of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretName string = 'AzureAdB2C--SignupAdminClientId'

@description('The value of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretValue string

@description('The name of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretName string = 'AzureAdB2C--ClientSecret'

@description('The value of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretValue string

@description('The name of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretName string = 'AzureAdB2C--TenantId'

@description('The value of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretValue string

@description('The name of the Key Vault.')
param keyVaultName string

@description('The location for all resources.')
param location string

@description('The name of the Permissions Api Certificate Key Vault Secret.')
param permissionsApiCertificateSecretName string = 'KeyVault--PermissionsApiCertName'

@description('The value of the Permissions Api Certificate Key Vault Secret.')
param permissionsApiCertificateSecretValue string

@description('The name of the Permissions Api SSL Thumbprint Key Vault Secret.')
param permissionsApiSslThumbprintSecretName string = 'AppSettings--SSLCertThumbprint'

@description('The value of the Permissions Api SSL Thumbprint Key Vault Secret.')
param permissionsApiSslThumbprintSecretValue string

@description('The name of the Permissions SQL Connection String Key Vault Secret.')
param permissionsSqlConnectionStringSecretName string = 'ConnectionStrings--PermissionsContext'

@description('The value of the Permissions SQL Connection String Key Vault Secret.')
param permissionsSqlConnectionStringSecretValue string

// Resource - Key Vault
//////////////////////////////////////////////////
resource keyVault 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    enabledForTemplateDeployment: true
    enableSoftDelete: false
    // softDeleteRetentionInDays: 7
    // enablePurgeProtection: false
    tenantId: subscription().tenantId
    publicNetworkAccess: 'enabled'
    accessPolicies: []
  }
}

// Resource - Key Vault - Secret - Permissions Api SSL Thumbprint
//////////////////////////////////////////////////
resource permissionsApiSslThumbprintSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: permissionsApiSslThumbprintSecretName
  properties: {
    value: permissionsApiSslThumbprintSecretValue
  }
}

// Resource - Key Vault - Secret - Permissions SQL Connection String
//////////////////////////////////////////////////
resource permissionsSqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: permissionsSqlConnectionStringSecretName
  properties: {
    value: permissionsSqlConnectionStringSecretValue
  }
}

// Resource - Key Vault - Secret - Permissions Api Certificate
//////////////////////////////////////////////////
resource permissionsApiCertificateSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: permissionsApiCertificateSecretName
  properties: {
    value: permissionsApiCertificateSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Instance
//////////////////////////////////////////////////
resource azureAdB2cInstanceSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cInstanceSecretName
  properties: {
    value: azureAdB2cInstanceSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Domain
//////////////////////////////////////////////////
resource azureAdB2cDomainSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cDomainSecretName
  properties: {
    value: azureAdB2cDomainSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Admin API Client Id
//////////////////////////////////////////////////
resource azureAdB2cAdminApiClientIdSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cAdminApiClientIdSecretName
  properties: {
    value: azureAdB2cAdminApiClientIdSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Tenant Id
//////////////////////////////////////////////////
resource azureAdB2cTenantSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cTenantIdSecretName
  properties: {
    value: azureAdB2cTenantIdSecretValue
  }
}

// Resource - Key Vault - Secret - Admin SQL Connection String
//////////////////////////////////////////////////
resource adminSqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: adminSqlConnectionStringSecretName
  properties: {
    value: adminSqlConnectionStringSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Signup Admin Client Id
//////////////////////////////////////////////////
resource azureAdB2cSignupAdminClientIdSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSignupAdminClientIdSecretName
  properties: {
    value: azureAdB2cSignupAdminClientIdSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Signup Admin Client Secret
//////////////////////////////////////////////////
resource azureAdB2cSignupAdminClientSecretSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSignupAdminClientSecretSecretName
  properties: {
    value: azureAdB2cSignupAdminClientSecretSecretValue
  }
}


// Outputs
//////////////////////////////////////////////////
output keyVaultId string = keyVault.id
output keyVaultUri string = keyVault.properties.vaultUri

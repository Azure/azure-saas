// Parameters
//////////////////////////////////////////////////
@description('The name of the Admin SQL Connection String Key Vault Secret.')
param adminSqlConnectionStringSecretName string = 'admin-ConnectionStrings--TenantsContext'

@description('The value of the Admin SQL Connection String Key Vault Secret.')
param adminSqlConnectionStringSecretValue string

@description('The name of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretName string = 'admin-AzureAdB2C--ClientId'

@description('The value of the Azure AD B2C Admin Api Client Id Key Vault Secret.')
param azureAdB2cAdminApiClientIdSecretValue string

@description('The name of the Azure AD B2C Domain Key Vault Secret for the Admin Api.')
param azureAdB2cAdminApiDomainSecretName string = 'admin-AzureAdB2C--Domain'

@description('The name of the Azure AD B2C Domain Key Vault Secret for the SignupAdmin Site.')
param azureAdB2cSignupAdminDomainSecretName string = 'signupadmin-AzureAdB2C--Domain'

@description('The value of the Azure AD B2C Domain Key Vault Secret.')
param azureAdB2cDomainSecretValue string

@description('The name of the Azure AD B2C SaaS App Client Id Key Vault Secret.')
param azureAdB2cSaasAppClientIdSecretName string = 'saasapplication-AzureAdB2C--ClientId'

@description('The value of the Azure AD B2C SaaS App Client Id Key Vault Secret.')
param azureAdB2cSaasAppClientIdSecretValue string

@description('The name of the Azure AD B2C SaaS App Client Secret Key Vault Secret.')
param azureAdB2cSaasAppClientSecretSecretName string = 'saasapplication-AzureAdB2C--ClientSecret'

@description('The value of the Azure AD B2C SaaS App Client Secret Key Vault Secret.')
param azureAdB2cSaasAppClientSecretSecretValue string

@description('The name of the Azure AD B2C Domain Key Vault Secret for the Saas App Site.')
param azureAdB2cSaasAppDomainSecretName string = 'saasapplication-AzureAdB2C--Domain'

@description('The name of the Azure AD B2C Instance Key Vault Secret for the Saas App Site.')
param azureAdB2cSaasAppInstanceSecretName string = 'saasapplication-AzureAdB2C--Instance'

@description('The name of the Azure AD B2C Tenant Id Key Vault Secret for the SignupAdmin site.')
param azureAdB2cSaasAppTenantIdSecretName string = 'saasapplication-AzureAdB2C--TenantId'

@description('The name of the Azure AD B2C Instance Key Vault Secre for the Admin Api.')
param azureAdB2cAdminApiInstanceSecretName string = 'admin-AzureAdB2C--Instance'

@description('The name of the Azure AD B2C Instance Key Vault Secret for the SignupAdmin Site.')
param azureAdB2cSignupAdminInstanceSecretName string = 'signupadmin-AzureAdB2C--Instance'

@description('The value of the Azure AD B2C Instance Key Vault Secret.')
param azureAdB2cInstanceSecretValue string

@description('The name of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretName string = 'signupadmin-AzureAdB2C--ClientId'

@description('The value of the Azure AD B2C Signup Admin Client Id Key Vault Secret.')
param azureAdB2cSignupAdminClientIdSecretValue string

@description('The name of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretName string = 'signupadmin-AzureAdB2C--ClientSecret'

@description('The value of the Azure AD B2C Signup Admin Client Secret Key Vault Secret.')
param azureAdB2cSignupAdminClientSecretSecretValue string

@description('The name of the Azure AD B2C Tenant Id Key Vault Secret for the Admin Api.')
param azureAdB2cAdminApiTenantIdSecretName string = 'admin-AzureAdB2C--TenantId'

@description('The name of the Azure AD B2C Tenant Id Key Vault Secret for the SignupAdmin site.')
param azureAdB2cSignupAdminTenantIdSecretName string = 'signupadmin-AzureAdB2C--TenantId'

@description('The value of the Azure AD B2C Tenant Id Key Vault Secret.')
param azureAdB2cTenantIdSecretValue string

@description('The name of the Key Vault.')
param keyVaultName string

@description('The location for all resources.')
param location string

@description('The name of the Permissions Api Api Key Key Vault Secret.')
param permissionsApiApiKeySecretName string = 'admin-KeyVault--PermissionsApiApiKey'

@description('The value of the Permissions Api Api Key Key Vault Secret.')
param permissionsApiApiKeySecretValue string


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

// Resource - Key Vault - Secret - Permissions Api Certificate
//////////////////////////////////////////////////
resource permissionsApiApiKeySecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: permissionsApiApiKeySecretName
  properties: {
    value: permissionsApiApiKeySecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Instance Admin Api
//////////////////////////////////////////////////
resource azureAdB2cAdminApiInstanceSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cAdminApiInstanceSecretName
  properties: {
    value: azureAdB2cInstanceSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Instance SignupAdmin
//////////////////////////////////////////////////
resource azureAdB2cSignupAdminInstanceSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSignupAdminInstanceSecretName
  properties: {
    value: azureAdB2cInstanceSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Client ID SaaS App
//////////////////////////////////////////////////
resource azureAdB2cSaasAppClientIdSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSaasAppClientIdSecretName
  properties: {
    value: azureAdB2cSaasAppClientIdSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Client Secret SaaS App
//////////////////////////////////////////////////
resource azureAdB2cSaasAppClientSecretSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSaasAppClientSecretSecretName
  properties: {
    value: azureAdB2cSaasAppClientSecretSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Domain SaaS App
//////////////////////////////////////////////////
resource azureAdB2cSaasAppDomainSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSaasAppDomainSecretName
  properties: {
    value: azureAdB2cDomainSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Instance SaaS App
//////////////////////////////////////////////////
resource azureAdB2cSaasAppInstanceSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSaasAppInstanceSecretName
  properties: {
    value: azureAdB2cInstanceSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Instance SaaS App
//////////////////////////////////////////////////
resource azureAdB2cSaasAppTenantIdSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSaasAppTenantIdSecretName
  properties: {
    value: azureAdB2cTenantIdSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Domain Admin Api
//////////////////////////////////////////////////
resource azureAdB2cAdminApiDomainSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cAdminApiDomainSecretName
  properties: {
    value: azureAdB2cDomainSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C Domain SignupAdmin
//////////////////////////////////////////////////
resource azureAdB2cSignupAdminDomainSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSignupAdminDomainSecretName
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

// Resource - Key Vault - Secret - Azure AD B2C Admin API Tenant Id
//////////////////////////////////////////////////
resource azureAdB2cAdminApiTenantSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cAdminApiTenantIdSecretName
  properties: {
    value: azureAdB2cTenantIdSecretValue
  }
}

// Resource - Key Vault - Secret - Azure AD B2C SignupAdmin Tenant Id
//////////////////////////////////////////////////
resource azureAdB2cSignupAdminTenantSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: keyVault
  name: azureAdB2cSignupAdminTenantIdSecretName
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

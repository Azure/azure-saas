// Parameters
//////////////////////////////////////////////////
@description('The name of the Key Vault.')
param keyVaultName string

@description('The Principal Id of the Admin Api System Assigned Managed Identity.')
param adminApiPrincipalId string

@description('The ASDK modules to deploy.')
param modulesToDeploy object

@description('The Principal Id of the Signup Admin App Service System Assigned Managed Identity.')
param signupAdminAppServicePrincipalId string

// Existing Resource - Key Vault
//////////////////////////////////////////////////
resource keyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: keyVaultName
}

// Resource - Access Policy
//////////////////////////////////////////////////
resource accessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2021-11-01-preview' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      (modulesToDeploy.adminService) ? {
        objectId: adminApiPrincipalId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      } : {}
      (modulesToDeploy.signupAdminWeb) ? {
        objectId: signupAdminAppServicePrincipalId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      } : {}
    ]
  }
}

// Parameters
//////////////////////////////////////////////////
@description('The name of the Key Vault.')
param keyVaultName string

@description('The Principal Id of the Permissions Api System Assigned Managed Identity.')
param permissionApiPrincipalId string

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
      {
        objectId: permissionApiPrincipalId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

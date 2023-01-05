// Parameters
//////////////////////////////////////////////////
@description('The name of the Key Vault.')
param keyVaultName string

@description('The Principal Id of the Permissions Api User Assigned Managed Identity.')
param userAssignedIdentityName string

// Existing Resource - Key Vault
//////////////////////////////////////////////////
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
}

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

// Resource - Access Policy
//////////////////////////////////////////////////
resource accessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2022-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        objectId: userAssignedIdentity.properties.principalId
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

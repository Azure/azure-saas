param keyVaultName string
param objectId string = 'e917db92-6445-433e-ac97-2eab88ce737d'

param location string = resourceGroup().location
param tenantId string = subscription().tenantId

@description('Specifies the role the user will get with the secret in the vault. Valid values are: Key Vault Administrator, Key Vault Certificates Officer, Key Vault Crypto Officer, Key Vault Crypto Service Encryption User, Key Vault Crypto User, Key Vault Reader, Key Vault Secrets Officer, Key Vault Secrets User.')
@allowed([
  'Key Vault Owner'
  'Key Vault Administrator'
  'Key Vault Certificates Officer'
  'Key Vault Crypto Officer'
  'Key Vault Crypto Service Encryption User'
  'Key Vault Crypto User'
  'Key Vault Reader'
  'Key Vault Secrets Officer'
  'Key Vault Secrets User'
])
param roleOwner string = 'Key Vault Owner'

@description('Specifies the role the user will get with the secret in the vault. Valid values are: Key Vault Administrator, Key Vault Certificates Officer, Key Vault Crypto Officer, Key Vault Crypto Service Encryption User, Key Vault Crypto User, Key Vault Reader, Key Vault Secrets Officer, Key Vault Secrets User.')
@allowed([
  'Key Vault Owner'
  'Key Vault Administrator'
  'Key Vault Certificates Officer'
  'Key Vault Crypto Officer'
  'Key Vault Crypto Service Encryption User'
  'Key Vault Crypto User'
  'Key Vault Reader'
  'Key Vault Secrets Officer'
  'Key Vault Secrets User'
])
param roleAdmin string = 'Key Vault Administrator'

var roleIdMapping = {
  'Key Vault Owner': '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
  'Key Vault Administrator': '00482a5a-887f-4fb3-b363-3b7fe8e74483'
  'Key Vault Certificates Officer': 'a4417e6f-fecd-4de8-b567-7b0420556985'
  'Key Vault Crypto Officer': '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'
  'Key Vault Crypto Service Encryption User': 'e147488a-f6f5-4113-8e2d-b22465e65bf6'
  'Key Vault Crypto User': '12338af0-0e69-4776-bea7-57ae8d297424'
  'Key Vault Reader': '21090545-7ca7-4776-b22c-e363652d74d2'
  'Key Vault Secrets Officer': 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableRbacAuthorization: true
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: objectId
        permissions: {
          certificates: ['all']
          keys: ['all']
          secrets: ['all']
        }
      }
    ]
  }
}

resource kvRoleAssignmentOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roleIdMapping[roleOwner],objectId,keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleIdMapping[roleOwner])
    principalId: objectId
    principalType: 'User'
  }
}

resource kvRoleAssignmentAdmin 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roleIdMapping[roleAdmin],objectId,keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleIdMapping[roleAdmin])
    principalId: objectId
    principalType: 'User'
  }
}

output publicKey string = keyVault.properties.provisioningState

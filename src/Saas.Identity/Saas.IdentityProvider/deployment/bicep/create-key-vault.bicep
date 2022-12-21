param keyVaultName string
param userObjectId string

param location string = resourceGroup().location
param tenantId string = subscription().tenantId

var rolesJson = loadJsonContent('roles.json')
var roles = rolesJson.roles

var roleOwner = 'Key Vault Owner'
var roleAdmin = 'Key Vault Administrator'

resource identityKeyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
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
        objectId: userObjectId
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
  name: guid(roles[roleOwner],userObjectId,identityKeyVault.id)
  scope: identityKeyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleOwner])
    principalId: userObjectId
    principalType: 'User'
  }
}

resource kvRoleAssignmentAdmin 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[roleAdmin],userObjectId,identityKeyVault.id)
  scope: identityKeyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleAdmin])
    principalId: userObjectId
    principalType: 'User'
  }
}

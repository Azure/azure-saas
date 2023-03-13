param keyVaultName string
param userObjectId string

param location string = resourceGroup().location
param tenantId string = subscription().tenantId

var rolesJson = loadJsonContent('./IdentityFoundation/Module/roles.json')
var roles = rolesJson.roles

var roleOwner = 'Key Vault Owner'
var roleAdmin = 'Key Vault Administrator'
var roleSecretOfficer = 'Key Vault Secrets Officer'

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
  }
}

resource kvRoleAssignmentOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[roleOwner], userObjectId, keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleOwner])
    principalId: userObjectId
    principalType: 'User'
  }
}

resource kvRoleAssignmentSecretOfficer 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[roleSecretOfficer], userObjectId, keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleSecretOfficer])
    principalId: userObjectId
    principalType: 'User'
  }
}

resource kvRoleAssignmentAdmin 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(roles[roleAdmin], userObjectId, keyVault.id)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roles[roleAdmin])
    principalId: userObjectId
    principalType: 'User'
  }
}

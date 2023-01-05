
// @description('The name of the Key Vault.')
// param keyVaultName string

// @description('The name of the Azure App Configuration.')
// param appConfigurationName string

// @description('Azure App Configuration User Assigned Identity Name.')
// param appConfigUserAssignedIdentityName string

// @description('The name of the key to hold the value in Azure App Configuration.')
// param keyName string

// @description('The name of the key of the value held in Azure Key Vault.')
// param keyVaultKeyName string

// @description('Key type (secret or certificates) to store in Azure Key Vault.')
// @allowed([
//   'secret'
//   'certificate'
// ])
// param keyType string

// @description('The name of value to store.')
// param value string

// var isCertificate = keyType == 'certificate'

// var rolesJson = loadJsonContent('../roles.json')
// var roles = rolesJson.roles
// var keyVaultSecretUser = 'Key Vault Secrets User'

// resource appConfigUserAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
//   name: appConfigUserAssignedIdentityName
// }

// resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
//   name: appConfigurationName
// }

// resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
//   name: keyVaultName
// }

// resource keyVaultCertificateEntry 'Microsoft.Web/certificates@2022-03-01' existing = if (isCertificate) {
//   name: keyVaultKeyName
// }


// resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' existing = if (!isCertificate) {
//   name: keyVaultKeyName
// }

// resource appConfigurationCertificateEntry 'Microsoft.AppConfiguration/configurationStores/keyValues@2022-05-01' = if (isCertificate) {
//   parent:appConfig
//     name: replace(keyName, ':', '__')
//     properties: {
//       value: '{"uri":"${keyVaultCertificateEntry.properties.keyVaultId}"}'
//       contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
//     }
//   }

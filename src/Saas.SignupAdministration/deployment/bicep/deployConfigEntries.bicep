@description('Version')
param version string

@description('The name of the key vault')
param keyVaultName string

@description('The URI of the key vault.')
param keyVaultUri string

@description('Azure B2C Domain Name.')
param azureB2CDomain string

@description('Azure B2C Tenant Id.')
param azureB2cTenantId string

@description('Azure AD Instance')
param azureAdInstance string

@description('The Azure B2C Signed Out Call Back Path.')
param signedOutCallBackPath string

@description('The Azure B2C Sign up/in Policy Id.')
param signUpSignInPolicyId string

@description('The Client Id found on registered Permissions API app page.')
param clientId string

@description('User Identity Name')
param userAssignedIdentityName string

@description('App Configuration Name')
param appConfigurationName string

@description('Indicates the Authentication type for new identity')
param authenticationType string

@description('Type of the claim to use in the new Identity, works alongside built-in')
param roleClaimType string

@description('Name of the claim custom roles are in')
param sourceClaimType string

@description('The name of the certificate key.')
param certificateKeyName string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

var certificates = [
  {
    SourceType: keyVaultName
    KeyVaultUrl: keyVaultUri
    KeyVaultCertificateName: certificateKeyName
  }
]

var azureB2CKeyName = 'AzureB2C'
var signupAdminKeyName = 'SignupAdmin'
var claimToRoleTransformerKeyName = 'ClaimToRoleTransformer'

var appConfigStore = {
  appConfigurationName: appConfigurationName
  keyVaultName: keyVaultName
  userAssignedIdentityName: userAssignedIdentity.name
  label: version
  entries: [
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:KeyVaultCertificateReferences'
      value: ' ${string(certificates)}' // notice the space before the string, this is a necessary hack. https://github.com/Azure/bicep/issues/6167
      isSecret: false
      contentType: 'application/json'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:ClientId'
      value: clientId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:TenantId'
      value: azureB2cTenantId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:Domain'
      value: azureB2CDomain
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:Instance'
      value: azureAdInstance
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:Audience'
      value: clientId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:SignedOutCallbackPath'
      value: signedOutCallBackPath
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${signupAdminKeyName}:${azureB2CKeyName}:SignUpSignInPolicyId'
      value: signUpSignInPolicyId
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${claimToRoleTransformerKeyName}:AuthenticationType'
      value: authenticationType
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${claimToRoleTransformerKeyName}:RoleClaimType'
      value: roleClaimType
      isSecret: false
      contentType: 'text/plain'
    }
    {
      key: '${claimToRoleTransformerKeyName}:SourceClaimType'
      value: sourceClaimType
      isSecret: false
      contentType: 'text/plain'
    }
  ]
}

// Adding App Configuration entries
module appConfigurationSettings './../../../Saas.Lib/Saas.Bicep.Module/addConfigEntry.bicep' = [ for entry in appConfigStore.entries: {
  name: replace('Entry-${entry.key}', ':', '-')
  params: {
    appConfigurationName: appConfigStore.appConfigurationName
    userAssignedIdentityName: appConfigStore.userAssignedIdentityName
    keyVaultName: keyVaultName
    value: entry.value
    contentType: entry.contentType
    keyName: entry.key
    label: appConfigStore.label
    isSecret: entry.isSecret
  }
}]

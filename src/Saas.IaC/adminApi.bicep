// Parameters
//////////////////////////////////////////////////
@description('The App Service Plan ID.')
param appServicePlanId string

@description('The location for all resources.')
param location string

@description('The Admin Api name.')
param adminApiName string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The name of the Permissions Api Certificate Key Vault Secret.')
param permissionsApiCertificateSecretName string = 'KeyVault--PermissionsApiCertName'

@description('The Permissions Api host name.')
param permissionsApiHostName string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string

@description('The tag of the container image to deploy to the permissions api app service')
param adminApiContainerImageTag string

// Resource - Admin Api
//////////////////////////////////////////////////
resource adminApi 'Microsoft.Web/sites@2021-03-01' = {
  name: adminApiName
  location: location
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: 'DOCKER|${adminApiContainerImageTag}'
      appSettings: [
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: containerRegistryUrl
        }        
        {
          name: 'AzureAdB2C__SignedOutCallbackPath'
          value: '/signout/B2C_1A_SIGNUP_SIGNIN'
        }        
        {
          name: 'AzureAdB2C__SignUpSignInPolicyId'
          value: 'B2C_1A_SIGNUP_SIGNIN'
        }
        {
          name: 'KeyVault__Url'
          value: keyVaultUri
        }
        {
          name: 'KeyVault__PermissionsApiCertName'
          value: permissionsApiCertificateSecretName
        }
        {
          name: 'PermissionsApi__BaseUrl'
          value: permissionsApiHostName
        }
        {
          name: 'Logging__LogLevel__Default'
          value: 'Information'
        }
        {
          name: 'Logging__LogLevel__Microsoft.AspNetCore'
          value: 'Warning'
        }
      ]
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}


output adminApiHostName string = 'https://${adminApi.properties.defaultHostName}'
output systemAssignedManagedIdentityPrincipalId string = adminApi.identity.principalId

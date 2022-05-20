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
          name: 'AllowedHosts'
          value: '*'
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

// Resource - Admin Api - Deployment
//////////////////////////////////////////////////
// resource adminApiDeployment 'Microsoft.Web/sites/extensions@2021-03-01' = {
//   parent: adminApi
//   name: 'MSDeploy'
//   properties: {
//     packageUri: 'https://stsaasdev001.blob.${environment().suffixes.storage}/artifacts/saas-provider/Saas.Provider.Web.zip?sv=2020-04-08&st=2021-06-07T19%3A23%3A20Z&se=2022-06-08T19%3A23%3A00Z&sr=c&sp=rl&sig=kNf0qwTfaCJg02xYeUHlfmHOJvI1bGU1HftjUJ5hl5o%3D'
//   }
// }

// Outputs
//////////////////////////////////////////////////
output adminApiHostName string = 'https://${adminApi.properties.defaultHostName}'
output systemAssignedManagedIdentityPrincipalId string = adminApi.identity.principalId

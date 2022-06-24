// Parameters
//////////////////////////////////////////////////
@description('The Admin Api host name.')
param adminApiHostName string

@description('Scopes to authorize user for the admin service.')
param adminApiScopes string

@description('The base url for the app registration that the scopes belong to.')
param adminApiScopeBaseUrl string

@description('The App Service Plan ID.')
param appServicePlanId string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The location for all resources.')
param location string

@description('The Signup Admin App Service name.')
param signupAdminAppServiceName string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string

@description('The tag of the container image to deploy to the permissions api app service')
param signupAdminApiContainerImageTag string

// Resource - Signup Admin App Service
//////////////////////////////////////////////////
resource signupAdminAppService 'Microsoft.Web/sites@2021-03-01' = {
  name: signupAdminAppServiceName
  location: location
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: 'DOCKER|${signupAdminApiContainerImageTag}'
      appSettings: [
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: containerRegistryUrl
        }      
        {
          name: 'AppSettings__AdminServiceBaseUrl'
          value: adminApiHostName
        }
        {
          name: 'AppSettings__AdminServiceScopeBaseUrl'
          value: adminApiScopeBaseUrl
        }
        {
          name: 'AppSettings__AdminServiceScopes'
          value: adminApiScopes
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

// Resource - Signup Admin App Service - Deployment
//////////////////////////////////////////////////
// resource signupAdminAppServiceDeployment 'Microsoft.Web/sites/extensions@2021-03-01' = {
//   parent: signupAdminAppService
//   name: 'MSDeploy'
//   properties: {
//     packageUri: 'https://stsaasdev001.blob.${environment().suffixes.storage}/artifacts/saas-provider/Saas.Provider.Web.zip?sv=2020-04-08&st=2021-06-07T19%3A23%3A20Z&se=2022-06-08T19%3A23%3A00Z&sr=c&sp=rl&sig=kNf0qwTfaCJg02xYeUHlfmHOJvI1bGU1HftjUJ5hl5o%3D'
//   }
// }

// Outputs
//////////////////////////////////////////////////
output signupAdminAppServiceHostName string = signupAdminAppService.properties.defaultHostName
output systemAssignedManagedIdentityPrincipalId string = signupAdminAppService.identity.principalId

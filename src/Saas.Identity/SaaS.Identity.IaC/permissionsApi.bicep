// Parameters
//////////////////////////////////////////////////
@description('The App Service Plan ID.')
param appServicePlanId string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The location for all resources.')
param location string

@description('The Permissions Api name.')
param permissionsApiName string

@description('The tag of the container image to deploy to the permissions api app service')
param permissionsApiContainerImageTag string

// Resource - Permissions Api
//////////////////////////////////////////////////
resource permissionsApi 'Microsoft.Web/sites@2021-03-01' = {
  name: permissionsApiName
  location: location
  kind: 'container'
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    clientCertEnabled: true
    clientCertMode: 'Required'
    siteConfig: {
      linuxFxVersion: permissionsApiContainerImageTag
      appSettings: [
        {
          name: 'KeyVault__Url'
          value: keyVaultUri
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

// Resource - Permissions Api - Deployment
//////////////////////////////////////////////////
// resource permissionsApiDeployment 'Microsoft.Web/sites/extensions@2021-03-01' = {
//   parent: permissionsApi
//   name: 'MSDeploy'
//   properties: {
//     packageUri: 'https://stsaasdev001.blob.${environment().suffixes.storage}/artifacts/saas-provider/Saas.Provider.Web.zip?sv=2020-04-08&st=2021-06-07T19%3A23%3A20Z&se=2022-06-08T19%3A23%3A00Z&sr=c&sp=rl&sig=kNf0qwTfaCJg02xYeUHlfmHOJvI1bGU1HftjUJ5hl5o%3D'
//   }
// }

// Outputs
//////////////////////////////////////////////////
output permissionsApiHostName string = permissionsApi.properties.defaultHostName
output systemAssignedManagedIdentityPrincipalId string = permissionsApi.identity.principalId

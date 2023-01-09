// Parameters
//////////////////////////////////////////////////
@description('Version')
param version string

@description('The App Service Plan ID.')
param appServicePlanId string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The location for all resources.')
param location string

@description('The Permissions Api name.')
param permissionsApiName string

@description('The URL for the container registry to pull the docker images from')
param containerRegistryUrl string

@description('The tag of the container image to deploy to the permissions api app service')
param permissionsApiContainerImageTag string

@description('Azure App Configuration User Assigned Identity Name.')
param userAssignedIdentityName string

@description('The name of the Azure App Configuration.')
param appConfigurationName string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

// Resource - Permissions Api
//////////////////////////////////////////////////
resource permissionsApi 'Microsoft.Web/sites@2022-03-01' = {
  name: permissionsApiName
  location: location
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      linuxFxVersion: 'DOCKER|${permissionsApiContainerImageTag}'    
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${userAssignedIdentity.id}': {} }
  }
  resource appsettings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      Version: version
      DOCKER_REGISTRY_SERVER_URL: containerRegistryUrl
      Logging__LogLevel__Default: 'Information'
      Logging__LogLevel__Microsoft__AspNetCore: 'Warning'
      KeyVault__Url: keyVaultUri
      ASPNETCORE_ENVIRONMENT: 'Development'
      UserAssignedManagedIdentityClientId: userAssignedIdentity.properties.clientId
      AppConfiguration__Endpoint : appConfig.properties.endpoint
    }
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
// output managedIdentityPrincipalId string = permissionsApi.identity.principalId

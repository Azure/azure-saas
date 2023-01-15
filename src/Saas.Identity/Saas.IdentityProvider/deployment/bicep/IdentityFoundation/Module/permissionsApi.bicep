// Parameters
//////////////////////////////////////////////////
@description('Version')
param version string

@description('The App Service Plan ID.')
param appServicePlanName string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The location for all resources.')
param location string

@description('The Permissions Api name.')
param permissionsApiName string

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

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: 'S1'
  }
  properties: {
    reserved: true
  }
}

resource permissionsApi 'Microsoft.Web/sites@2022-03-01' = {
  name: permissionsApiName
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.name
    httpsOnly: true
    siteConfig: {
      alwaysOn: true 
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
      Logging__LogLevel__Default: 'Information'
      Logging__LogLevel__Microsoft__AspNetCore: 'Warning'
      KeyVault__Url: keyVaultUri
      ASPNETCORE_ENVIRONMENT: 'Development'
      UserAssignedManagedIdentityClientId: userAssignedIdentity.properties.clientId
      AppConfiguration__Endpoint : appConfig.properties.endpoint
    }
  }
}

resource permissionsApiStagingSlot 'Microsoft.Web/sites/slots@2022-03-01' = {
  name: 'PermissionsApi-Staging'
  parent: permissionsApi
  location: location
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.name
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { '${userAssignedIdentity.id}': {} }
  }
}

// resource permissionsApiSlotsConfig 'Microsoft.Web/sites/slots/config@2022-03-01' = {
//   name: 'web'
//   parent: permissionsApiSlots
//   properties: {
//     WEBSITE_RUN_FROM_PACKAGE: '1'
//   }
// }

// resource permissionSrcControls 'Microsoft.Web/sites/sourcecontrols@2022-03-01' = {  
//   name: 'web'
//   parent: permissionsApi
//   properties: {  
//     isGitHubAction: true
//     repoUrl: gitRepoUrl 
//     branch: gitBranch 
//     deploymentRollbackEnabled: true
//     isManualIntegration: false
//   }  
// } 

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
output appServicePlanName string = appServicePlan.name

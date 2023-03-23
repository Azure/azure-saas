@description('The App Service Plan ID.')
param appServicePlanName string

@description('The location for all resources.')
param location string

@description('Azure App Configuration User Assigned Identity Name.')
param userAssignedIdentityName string


@description('The name of the Log Analytics Workspace used by Application Insigths.')
param logAnalyticsWorkspaceName string

@description('The name of the Automation Account.')
param automationAccountName string

@description('The name of Application Insights.')
param applicationInsightsName string

@description('App Service Plan OS')
@allowed([
  'linux'
  'windows'
])
param appServicePlanOS string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { 
      '${userAssignedIdentity.id}': {} 
    }
  }
}

resource automationAccount 'Microsoft.Automation/automationAccounts@2022-08-08' = {
  name: automationAccountName
  location: location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}

var automationAccountLinkedWorkspaceName = 'Automation'

resource automationAccountLinkedWorkspace 'Microsoft.OperationalInsights/workspaces/linkedServices@2020-08-01' = {
  name: automationAccountLinkedWorkspaceName
  parent: logAnalyticsWorkspace
  properties: {
    resourceId: automationAccount.id
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  kind: appServicePlanOS
  sku: {
    name: 'S1'
  }
  properties: {
    reserved: ((appServicePlanOS == 'linux') ? true : false)
  }
}

// resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
//   name: appConfigurationName
// }

// // metadata :[
// //   {
// //     name:'CURRENT_STACK'
// //     value:'dotnetcode'
// //   }
// // ]

// resource permissionsApi 'Microsoft.Web/sites@2022-03-01' = {
//   name: permissionsApiName
//   location: location
//   kind: 'app,windows'
//   properties: {
//     serverFarmId: appServicePlan.name
//     httpsOnly: true
//     // clientCertEnabled: true // https://learn.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth?tabs=bicep
//     clientCertMode: 'Required' 
//     siteConfig: {
//       ftpsState: 'FtpsOnly'
//       alwaysOn: true 
//       http20Enabled: true
//       keyVaultReferenceIdentity: userAssignedIdentity.id // Must specify this when using User Assigned Managed Identity. Read here: https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli#access-vaults-with-a-user-assigned-identity
//       detailedErrorLoggingEnabled: true
//       netFrameworkVersion: 'v7.0'      
//       // linuxFxVersion: 'DOTNETCORE|7.0'
//     }
//   }
//   identity: {
//     type: 'UserAssigned'
//     userAssignedIdentities: { 
//       '${userAssignedIdentity.id}': {} 
//     }
//   }
//   resource appsettings 'config@2022-03-01' = {
//     name: 'appsettings'
//     properties: {
//       Version: 'ver${version}'
//       Logging__LogLevel__Default: 'Information'
//       Logging__LogLevel__Microsoft__AspNetCore: 'Warning'
//       KeyVault__Url: keyVaultUri
//       ASPNETCORE_ENVIRONMENT: environment
//       UserAssignedManagedIdentityClientId: userAssignedIdentity.properties.clientId
//       AppConfiguration__Endpoint : appConfig.properties.endpoint
//       APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.properties.ConnectionString // https://learn.microsoft.com/en-us/azure/azure-monitor/app/migrate-from-instrumentation-keys-to-connection-strings
//       ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
//     }
//   }
// }

// resource diagnosticsSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
//   name: 'string'
//   scope: permissionsApi
//   properties: {
//     logs: [
//       {
//         categoryGroup: 'allLogs'
//         enabled: true
//         retentionPolicy: {
//           days: 7
//           enabled: true
//         }
//       }
//     ]
//     metrics: [
//       {
//         category: 'AllMetrics'
//         enabled: true
//       }
//     ]
//     workspaceId: logAnalyticsWorkspace.id
//   }
// }

// // resource permissionsApiStagingSlot 'Microsoft.Web/sites/slots@2022-03-01' = {
// //   name: 'PermissionsApi-Staging'
// //   parent: permissionsApi
// //   location: location
// //   kind: 'app,linux'
// //   properties: {
// //     serverFarmId: appServicePlan.name
// //     httpsOnly: true
// //     siteConfig: {
// //       alwaysOn: true 
// //       linuxFxVersion: 'DOTNETCORE|7.0'
// //       http20Enabled: true
// //     }
// //   }
// //   identity: {
// //     type: 'UserAssigned'
// //     userAssignedIdentities: { '${userAssignedIdentity.id}': {} }
// //   }
// //   resource appsettings 'config@2022-03-01' = {
// //     name: 'appsettings'
// //     properties: {
// //       Version: version
// //       Logging__LogLevel__Default: 'Information'
// //       Logging__LogLevel__Microsoft__AspNetCore: 'Warning'
// //       KeyVault__Url: keyVaultUri
// //       ASPNETCORE_ENVIRONMENT: 'Development'
// //       UserAssignedManagedIdentityClientId: userAssignedIdentity.properties.clientId
// //       AppConfiguration__Endpoint : appConfig.properties.endpoint
// //     }
// //   }
// //   resource metadata 'config@2022-03-01' = {
// //     name: 'metadata'
// //     properties: {
// //       CURRENT_STACK: 'dotnet'      
// //     }
// //   }
// // }

// // Resource - Permissions Api - Deployment
// //////////////////////////////////////////////////
// // resource permissionsApiDeployment 'Microsoft.Web/sites/extensions@2021-03-01' = {
// //   parent: permissionsApi
// //   name: 'MSDeploy'
// //   properties: {
// //     packageUri: 'https://stsaasdev001.blob.${environment().suffixes.storage}/artifacts/saas-provider/Saas.Provider.Web.zip?sv=2020-04-08&st=2021-06-07T19%3A23%3A20Z&se=2022-06-08T19%3A23%3A00Z&sr=c&sp=rl&sig=kNf0qwTfaCJg02xYeUHlfmHOJvI1bGU1HftjUJ5hl5o%3D'
// //   }
// // }

// // Outputs
// //////////////////////////////////////////////////
// output permissionsApiHostName string = permissionsApi.properties.defaultHostName
output appServicePlanName string = appServicePlan.name

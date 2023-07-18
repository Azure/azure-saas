@description('The SaaS Signup Administration web site name.')
param appServiceName string

@description('Version')
param version string

@description('Environment')
@allowed([
  'Development'
  'Staging'
  'Production'
])
param environment string

@description('The App Service Plan ID.')
param appServicePlanName string

@description('The Uri of the Key Vault.')
param keyVaultUri string 

@description('The location for all resources.')
param location string

@description('Azure App Configuration User Assigned Identity Name.')
param userAssignedIdentityName string

@description('The name of the Azure App Configuration.')
param appConfigurationName string

@description('The name of the Log Analytics Workspace used by Application Insigths.')
param logAnalyticsWorkspaceName string

@description('The name of Application Insights.')
param applicationInsightsName string

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' existing = {
  name: logAnalyticsWorkspaceName
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' existing = {
  name: appServicePlanName
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2022-05-01' existing = {
  name: appConfigurationName
}

resource signupAdministrationWeb 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'app,windows'
  properties: {
    serverFarmId: appServicePlan.name
    httpsOnly: true
    // clientCertEnabled: true // https://learn.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth?tabs=bicep
    clientCertMode: 'Required' 
    siteConfig: {
      ftpsState: 'FtpsOnly'
      alwaysOn: true 
      http20Enabled: true
      keyVaultReferenceIdentity: userAssignedIdentity.id // Must specify this when using User Assigned Managed Identity. Read here: https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli#access-vaults-with-a-user-assigned-identity
      detailedErrorLoggingEnabled: true
      netFrameworkVersion: 'v7.0'      
      // linuxFxVersion: 'DOTNETCORE|7.0'
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: { 
      '${userAssignedIdentity.id}': {} 
    }
  }
  resource appsettings 'config@2022-03-01' = {
    name: 'appsettings'
    properties: {
      Version: 'ver${version}'
      Logging__LogLevel__Default: 'Information'
      Logging__LogLevel__Microsoft__AspNetCore: 'Warning'
      KeyVault__Url: keyVaultUri
      ASPNETCORE_ENVIRONMENT: environment
      UserAssignedManagedIdentityClientId: userAssignedIdentity.properties.clientId
      AppConfiguration__Endpoint : appConfig.properties.endpoint
      APPLICATIONINSIGHTS_CONNECTION_STRING: applicationInsights.properties.ConnectionString // https://learn.microsoft.com/en-us/azure/azure-monitor/app/migrate-from-instrumentation-keys-to-connection-strings
      ApplicationInsightsAgent_EXTENSION_VERSION: '~2'
    }
  }
}

resource diagnosticsSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'string'
  scope: signupAdministrationWeb
  properties: {
    logs: [
      {
        categoryGroup: 'allLogs'
        enabled: true
        retentionPolicy: {
          days: 7
          enabled: true
        }
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
    workspaceId: logAnalyticsWorkspace.id
  }
}

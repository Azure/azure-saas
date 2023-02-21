@description('The SaaS Signup Administration web site name.')
param saasapp string

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

module signupAdministrationWebApp './../../../Saas.Lib/Saas.Bicep.Module/appServiceModuleWithObservability.bicep' = {
  name: 'signupAdministrationWebApp'
  params: {
    appServiceName: saasapp
    version: version
    environment: environment
    appServicePlanName: appServicePlanName
    keyVaultUri: keyVaultUri
    location: location
    userAssignedIdentityName: userAssignedIdentityName
    appConfigurationName: appConfigurationName
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    applicationInsightsName: applicationInsightsName
  }
}


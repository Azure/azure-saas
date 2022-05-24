// Parameters
//////////////////////////////////////////////////
@description('The location for all resources.')
param location string

@description('The App Service Plan name.')
param appServicePlanName string

// Resource - App Service Plan
//////////////////////////////////////////////////
resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
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

// Outputs
//////////////////////////////////////////////////
output appServicePlanId string = appServicePlan.id

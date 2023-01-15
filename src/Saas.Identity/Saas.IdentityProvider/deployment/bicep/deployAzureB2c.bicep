
param location string
param countryCode string
param displayName string
param name string
param skuName string
param tier string

resource b2cDirectory 'Microsoft.AzureActiveDirectory/b2cDirectories@2021-04-01' = {
  name: name
  location: location
  sku: {
    name: skuName
    tier: tier
  }
  properties: {
    createTenantProperties: {
      countryCode: countryCode
      displayName: displayName
    }
  }
}

output tenantId string = b2cDirectory.properties.tenantId

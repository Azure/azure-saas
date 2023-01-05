@description('Location of the resource group')
param location string = resourceGroup().location

resource secretGenerator 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'password-generate'
  location: location
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.42.0'
    retentionInterval: 'PT1H' // retain the script output for 1 hour. Allowed range is 1-26 hours.
    // timeout: 'PT5M' // timeout after 1 hour. Allowed range is 5-120 minutes.
    scriptContent: loadTextContent('password-generate.sh')
    cleanupPreference: 'OnSuccess' // delete the script output on successful execution.
  }
}

output secret string = secretGenerator.properties.outputs.secret

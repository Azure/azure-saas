// Parameters
//////////////////////////////////////////////////
@description('The location for all resources.')
param location string

@description('The name of the Logic App.')
param logicAppName string

// Resource - Logic
//////////////////////////////////////////////////
resource logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: logicAppName
  location: location
  properties: {
    definition: {
      '$schema': 'https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#'
      contentVersion: '1.0.0.0'
      parameters: {}
      triggers: {
        manual: {
          type: 'Request'
          kind: 'Http'
          inputs: {
            schema: {
              properties: {
                HTML: {
                  type: 'string'
                }
                emailFrom: {
                  type: 'string'
                }
                emailTo: {
                  type: 'string'
                }
                emailToName: {
                  type: 'string'
                }
                subject: {
                  type: 'string'
                }
              }
              type: 'object'
            }
          }
        }
      }
      actions: {
        Response: {
          runAfter: {}
          type: 'Response'
          kind: 'Http'
          inputs: {
            body: '@triggerBody()?[\'HTML\']'
            statusCode: 200
          }
        }
      }
      outputs: {}
    }
  }
}

// Outputs
//////////////////////////////////////////////////
output logicAppCallbackURL string = listCallbackURL('${logicApp.id}/triggers/manual', logicApp.apiVersion).value

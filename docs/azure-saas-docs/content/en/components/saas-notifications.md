---
type: docs
title: "SaaS.Notifications"
weight: 100
---

## Overview

The SaaS.Notifications module is a relatively simple [Azure Logic App](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-overview) that gets deployed to enable email sending from the rest of the solution. It is deployed with an HTTP trigger that takes in a JSON payload with fields required to send an email.

## Design
### Dependencies

- Email provider of choice, once configured
### Consumers

- [SaaS.SignupAdministration.Web](../signup-administration)

## Logic App Configuration

### Input
The logic app comes pre-configured with a default JSON schema with some common email data fields. You may edit this to your liking by going to the logic app designer for the logic app that is deployed in your environment. Here is the default JSON schema that gets deployed: 

```json
{
    "properties": {
        "HTML": {
            "type": "string"
        },
        "emailFrom": {
            "type": "string"
        },
        "emailTo": {
            "type": "string"
        },
        "emailToName": {
            "type": "string"
        },
        "subject": {
            "type": "string"
        }
    },
    "type": "object"
}
```

### Email Provider
By default, the logic app that gets deployed does not come with an email provider configured. If you POST to the endpoint, the logic app will take in the data and simply return a 200 OK response. To enable actual email sending, you must first configure an email provider [connector](https://docs.microsoft.com/en-us/connectors/connector-reference/connector-reference-logicapps-connectors).

To do this, follow these instructions:
1. Navigate to the deployed logic app and click into the "Logic app designer" menu
2. Click the + button underneath the defined HTTP trigger and click "Add an action"
![](/azure-saas/images/logic-app-1.png)
3. Search for your email connector of choice and choose the action for sending an email (ex, for Office 365 it is "Send an Email (V2)")
4. Fill in the required fields with data from the "Dynamic content" section. This will pass data from the input HTTP trigger to the email connector
![](/azure-saas/images/logic-app-2.png)
5. Click "Save"
6. You may now test using the application, or by clicking "Run trigger" at the top of the logic app designer


If you'd like more information on how this works, check out some additional reading on this subject here: 

- [Connect to Office 365 Outlook using Azure Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-create-api-office365-outlook)
- [Connect to SendGrid from Azure Logic Apps](https://docs.microsoft.com/en-us/azure/connectors/connectors-create-api-office365-outlook)
- [Tutorial: Send email and invoke other business processes from App Service](https://docs.microsoft.com/en-us/azure/app-service/tutorial-send-email?tabs=dotnet)
# SaaS Provider Web App

The Saas Provider Web App is the marketing, onboarding and productivity component of Azure Saas for customers of your service.  This should be thought of as the equivalent to https://www.office365.com where anonymous users are able to navigate to, explore the various service offerings / pricing tiers and onboard.  

[![Deploy to Azure](https://www.azuresaas.net/assets/images/deploy-to-azure.svg)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-saas%2Fmain%2Fsrc%2FSaas.Provider%2FSaas.Provider.Web.Deployment%2Fazuredeploy.json)

Demo SaaS Service:  https://www.azuresaas.net

## Onboarding Flow
The onboarding flow of the SaaS Provider Web App is a series of steps which persists the data of each step to Azure Cosmos DB upon submit of the given step.


### Steps
1. Enter Email Address
	- Creates initial Identity on ASP.NET user account
2. Enter Organization Name
3. Enter Organization Category
4. Select your Plan
	- If 'Free' plan is selected, next step (billing) is skipped
5. Enter billing information using Stripe payments
	- Digital payments with Apple Pay, Google Pay, credit cards, etc.
6. Creating your Subscriber
	- Posts to Onboarding API to create new tenant
7. Confirmation
	- User is prompted to enter Password to complete Identity on ASP.NET user account
	- User is sent email to validate email address

<img src="https://stsaasprod001.blob.core.windows.net/assets/images/saas-provider-onboarding-flow.png">

## Azure Cosmos DB
 The onboarding flow writes directly to Azure Cosmos DB for single-digit millisecond response times.  The process generates a unique GUID unique identifier of the given user's flow and updates the JSON document with each property as the user proceeds through the six step process.

## Identity on ASP.NET Core
Upon entry of the first step of the onboarding flow (enter email address), the CreateController logic checks if an account exists with the given user name (email address).  If an account does not exist with the email address, an Identity account is created with just a user name.  Storing the email address allows for subsequent communication with the user in the case that they do not complete the onboarding flow.

If the user does complete the onboarding flow, the final confirmation step will include a field to enter a password to complete their account creation.  
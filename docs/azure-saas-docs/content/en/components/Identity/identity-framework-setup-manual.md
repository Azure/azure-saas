---
type: docs
title: "Identity Framework Setup - Manual"
weight: 5000
toc_hide: true
---

On this page, you will find instructions for how to manually setup the Identity Framework through a combination of the Azure dashboard as well as relevant PowerShell commands

# Setup Identity Framework - Manual (Advanced)

## 1. Requirements

- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/)
- [Open SSL](https://www.openssl.org/)
  - Note: An OpenSSL executable is also bundled with Git for Windows at `C:\Program Files\Git\usr\bin\openssl.exe`
- [Visual Studio Code](https://code.visualstudio.com/)
   - Required Extensions:
   - [Azure AD B2C Custom Policy Extension](https://marketplace.visualstudio.com/items?itemName=AzureADB2CTools.aadb2c)

## 2. Instructions

### Step 1. Create B2C Tenant in Azure

- From the dashboard, click `Create a Resource` and select `Azure Active Directory B2C`
- Note: A reference is registered in your subscription at time of creation to associate the tenant with your subscription.

![(Azure Add Service Screenshot)](/azure-saas/images/identity-framework-manual-step-1-adding-service.png)

---

### Step 2. Log into newly created Tenant

- Click on `Open B2C Tenant` quicklink to immediately change directory to your new Tenant

![(Switch Directory Screenshot)](/azure-saas/images/identity-framework-manual-step-2-switch-tenant-quicklink.png)

---

### Step 3. (Optional) Invite collaborators to the new Tenant

Note: Collaborators are other developers you wish to help manage your services, not customers

- Go to the `Azure AD B2C` dashboard while logged into your new Tenant, click `Users` in the lefthand navbar
- After navigating, you may create or invite guest users into your Tenant allowing them access

![(Tenant User Management Screenshot)](/azure-saas/images/identity-framework-manual-step-3-invite-collaborators.png)

---

### Step 4. Create necessary app registrations in Azure AD B2C Tenant

- From the Tenant `Azure AD B2C` dashboard, click `App registrations`
- Add new registrations corresponding to the following list of modules
- Instructions for adding client secrets, API scopes, and API permissions are detailed below

![(Registered Apps Screenshot)](/azure-saas/images/identity-framework-manual-step-4-app-registration.png)

#### Modules

- asdk-admin-api
  - Display Name: asdk-admin-api
  - Account Type: Accounts in any identity provider or organizational directory
  - Grant Admin Consent to openid and offline_access_permissions: true
  - API Scopes:

    | Scope Name | Description |
    | - | - |
    | tenant.read | Read a customer's own Tenant data |
    | tenant.global.read | Admin-level read permissions for all Tenants |
    | tenant.write | Alter a customer's own Tenant data |
    | tenant.global.write | Admin-level write permissions for all Tenants |
    | tenant.delete | Delete a customer's own Tenant data |
    | tenant.global.delete | Admin-level delete permissions for all Tenants |

- asdk-signupadmin-app
  - Display Name: asdk-saas-app
  - Account Type: Accounts in any identity provider or organizational directory
  - Redirect URI: [Single-page application] `https://appsignup{providerName}{environmentName}.azurewebsites.net/signin-oidc`
  - Grant Admin Consent to openid and offline_access_permissions: true
  - Create Client Secrets?: Yes
  - Required API Permissions: 
    - tenant.read
    - tenant.global.read
    - tenant.write
    - tenant.global.write
    - tenant.delete
    - tenant.global.delete
- asdk-saas-app
  - Display Name: asdk-saas-app
  - Account Type: Accounts in any identity provider or organizational directory
  - Redirect URI: [Single-page application] `https://appapplication{providerName}{environmentName}.azurewebsites.net/signin-oidc`
  - Grant Admin Consent to openid and offline_access_permissions: true
  - Create Client Secrets?: Yes
  - Required API Permissions: 
    - tenant.read
- asdk-permissions-api
  - Display Name: asdk-permissions-api
  - Account Type: Accounts in any identity provider or organizational directory
  - Grant Admin Consent to openid and offline_access_permissions: true
- IdentityExperienceFramework
  - Display Name: IdentityExperienceFramework
  - Account Type: Accounts in any identity provider or organizational directory
  - Grant Admin Consent to openid and offline_access_permissions: true
- ProxyIdentityExperienceFramework
  - Display Name: ProxyIdentityExperienceFramework
  - Account Type: Accounts in any identity provider or organizational directory
  - Grant Admin Consent to openid and offline_access_permissions: true

#### Module Client Secrets

- From the `App registration` view
- For the apps listed below, navigate to them, then navigate to `Certificates & secrets` in their navbars
- Add a new client secret for later reference, giving it an appropriate expiration and description

![(Client Secrets Screenshot)](/azure-saas/images/identity-framework-manual-step-4-module-client-secrets.png)

#### Module API Scopes

- From the `App registration` view
- For the apps that need a scope exposed (listed above), navigate to them, then navigate to `Expose an API` in their navbars
- The first time you add a scope, you will be prompted to define the `Application ID URI`, do so and save the domain and identifier each independently for later reference

![(Expose an API Scopes)](/azure-saas/images/identity-framework-manual-step-4-app-expose-api.png)

#### Module API Permissions
- From the `App registration` view
- For the apps listed below, navigate to them, then navigate to `API permissions` in their navbars
- Add the following API permissions on the associated module:
- asdk-saas-app
  - tenant.read
- asdk-signupadmin-app
  - tenant.read
  - tenant.global.read
  - tenant.write
  - tenant.global.write
  - tenant.delete
  - tenant.global.delete
  - Delete tenant global

---

### Step 5. Create self-signed certificate

>Note: A self-signed certificate is not recommended for production environments. Read more [here](https://azure.github.io/azure-saas/components/identity/permissions-service/#authentication).

- Create an RSA self-signed certificate to be referenced and uploaded to your Azure AD B2C Tenant
- The .pfx file will be uploaded in a later step to your Azure AD B2C to allow it access to integrate with your Permissions API
- OpenSSL with [Powershell Core (v7.0+)](https://github.com/PowerShell/PowerShell) Example:

```
$pswd= Read-Host -Prompt "Please enter a password to encrypt the self signed certificate with"

# Create a new certificate in .crt format with the subject name as *.azurewebsites.net. You could also provide a specific subject name of your permissions api.
openssl req -newkey rsa:4096 -x509 -sha256 -days 365 -nodes -out certificate.crt -keyout certificate.key -subj         "/CN=*.azurewebsites.net"

# Export the .crt and .key to a .pfx format
openssl pkcs12 -export -out selfSignedCertificate.pfx -inkey certificate.key -in certificate.crt -password pass:$pswd

# Encode pfx to base 64 string to reference in step 13
$pfxBytes = Get-Content "selfSignedCertificate.pfx" -AsByteStream
$pfxString = [System.Convert]::ToBase64String($pfxBytes)
```

---

### Step 6. Create Policy Keys in Azure AD B2C Tenant

![(Identity Experience Framework Screenshot)](/azure-saas/images/identity-framework-manual-step-6-identity-experience-framework.png)

- Find `Policy keys` in `Azure AD B2C Service` by navigating to `Identity Experience Framework`
- Click Add and select the following options to generate the system policy keys

![(Policy Keys Screenshot)](/azure-saas/images/identity-framework-manual-step-6-policy-keys.png)

- TokenSigningKeyContainer
  - Options: Generate
  - Name: TokenSigningKeyContainer
  - Key type: RSA
  - Key usage: Signature
- TokenEncryptionKeyContainer
  - Options: Generate
  - Name: TokenEncryptionKeyContainer
  - Key type: RSA
  - Key usage: Encryption
- RestApiClientCertificate
  - Options: Upload
  - Name: *.azurewebsites.net
  - File upload: (The .pfx created in step 5)
  - Password: (The password created in step 5)

---

### Step 7. Clone ASDK repo

- Clone the latest release of the [ASDK](https://github.com/Azure/azure-saas)

---

### Step 8. Initiate identity Bicep deployment with parameters from previous few steps

- Generate a standard [Bicep parameters file](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/parameter-files) using the parameters below
- Navigate to `Saas.Identity/Saas.Identity.IaC` for the `az deploy` to work correctly
- Deploy identity Bicep with the following parameters
  - You may reference or fill out `main.parameters.json` in the Saas.Identity.IaC directory
  - (Note: some parameters are referenced in step 13 and must match what is selected here, such as the chosen saasProviderName)
-      az deployment group create --name "IdentityBicepDeployment" --resource-group <#YourAzureResourceGroupName#> template-file ./main.bicep --parameters ./main.parameters.json

#### Parameters

| Parameter Name | Value |
| - | - |
| AppSettings:AdminServiceBaseUrl             | (URL for downstream admin service) |
| azureAdB2cDomainSecretValue| (Your Tenant domain found on your AD B2C dashboard)|
| azureAdB2cInstanceSecretValue| (The B2C login endpoint in format of https://(Tenant Name).b2clogin.com)|
| azureAdB2cTenantIdSecretValue| (Your Tenant Subscription ID found on your AD B2C dashboard)|
| azureAdB2cPermissionsApiClientIdSecretValue| (The Client ID found on your registered Permissions API app page)|
| azureAdB2cPermissionsApiClientSecretSecretValue| (The Client Secret Value created in step 8)|
| permissionsApiSslThumbprintSecretValue| (Thumbprint created in step 5)|
| saasProviderName| (Select a provider name. This name will be used to name the Azure Resources. (e.g. contoso, myapp). Max Length is 8 characters.)|
| saasEnvironment| (Select an environment name. (e.g. 'prd', 'stg', 'dev', 'tst'))|
| saasInstanceNumber| (Select an instance number. This number will be appended to most Azure Resources created. (e.g. 001, 002, 003))|
| sqlAdministratorLogin| (Select an admin account name used for resource creation)|
| sqlAdministratorLoginPassword| (Select a password for the admin account)

#### Example Parameters Json

```
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "AppSettings:AdminServiceBaseUrl": {
      "value": "asdk-demo-admin.com"
    },
    "azureAdB2cDomainSecretValue": {
      "value": "asdkdemotenant.onmicrosoft.com"
    },
    "azureAdB2cInstanceSecretValue": {
      "value": "https://asdkdemotenant.b2clogin.com"
    },
    "azureAdB2cTenantIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
    "azureAdB2cPermissionsApiClientIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
    "azureAdB2cPermissionsApiClientSecretSecretValue": {
      "value": "FooBar"
    },
    "permissionsApiSslThumbprintSecretValue": {
      "value": "FooBar"
    },
    "saasProviderName": {
      "value": "contoso"
    },
    "saasEnvironment": {
      "value": "tst"
    },
    "saasInstanceNumber": {
      "value": "001"
    },
    "sqlAdministratorLogin": {
      "value": "sqlAdmin"
    },
    "sqlAdministratorLoginPassword": {
      "value": "FooBar1"
    }
  }
}
```

---

### Step 9. Configure transform IEF Policies with config values from previous few steps

- Launch PowerShell and navigate to project directory `src\Saas.Identity\Saas.IdentityProvider\policies`
- Launch VS Code using:
-     code .
- Open or create `appsettings.json`
- Note: You may reference `sample.appsettings.json` for basic parameter setup 
- Complete the settings for the following attributes:

| Parameter Name | Value |
| - | - |
| Name | (Select a name for the environment (e.g. Development)) |
| Production | (a boolean indicating production status) |
| Tenant | (Name of Tenant) |
| PolicySettings:IdentityExperienceFrameworkAppId | (App ID of IdentityExperienceFramework app created in step 4) |
| PolicySettings:ProxyIdentityExperienceFrameworkAppId | (App ID of ProxyIdentityExperienceFramework app created in step 4) |
| PolicySettings:PermissionsAPIUrl | (URL for the permissions endpoint in the Permission module (e.g. (PermissionsApiFQDN)/api/CustomClaims/permissions)) | 
| PolicySettings:RolesAPIUrl | (URL for the roles endpoint in the Permission module (e.g. (PermissionsApiFQDN)/api/CustomClaims/roles)) |


- Open the VS Code command palette (shortcut: ctrl + shift + p)
- Execute `B2C Build All Policies`
- Navigate to `Environments` dropdown in VS Code Explorer for generated policies

![(VS Code Screenshot)](/azure-saas/images/identity-framework-manual-step-9-vscode-ief-policies.png)

---

### Step 10. Upload IEF Policies to Azure AD B2C Tenant

- In your Azure dashboard, navigate to `Azure AD B2C` service, then to `Identity Framework Experience` in the nav bar
- Select `Upload custom policy` and upload each of the generated policy files

![(Policy Upload Screenshot)](/azure-saas/images/identity-framework-manual-step-10-upload-policies.png)

---

### Step 11. Create parameters.json for input into main bicep deployment

- Generate a standard [Bicep parameters file](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/parameter-files) using the parameters below
- Place file into any folder (you will need it on step 2.b of the [Quick Start](https://azure.github.io/azure-saas/quick-start/#2b-deploying-to-azure---entire-solution) guide

#### Parameters

| Parameter Name | Value |
| - | - |
| adminApiScopes| (space delimited string of admin scope names (e.g. "test.scope tenant.delete tenant.global.delete tenant.global.read tenant.global.write tenant.read tenant.write"))|
| adminApiScopeBaseUrl| (Reference your Tenant domain and the registered Admin app identifier, format of| "https|//{your tenant name}.onmicrosoft.com/{admin app client id}")|
| azureAdB2cAdminApiClientIdSecretValue| (Admin Api Client Id found under its Registered App entry) |
| azureAdB2cDomainSecretValue| (Your Tenant domain found on your AD B2C dashboard)|
| azureAdB2cInstanceSecretValue| (The B2C login endpoint in format of| https|//(Tenant Name).b2clogin.com)|
| azureAdB2cSaasAppClientIdSecretValue|  (SaaS App Api Client Id found under its Registered App entry) |
| azureAdB2cSaasAppClientSecretSecretValue| (Secret value created in step 4)|
| azureAdB2cSignupAdminClientIdSecretValue| (SaaS App Api Client Id found under its Registered App entry) |
| azureAdB2cSignupAdminClientSecretSecretValue| (Secret value created in step 4)|
| azureAdB2cTenantIdSecretValue| (Your Tenant Subscription ID found on your AD B2C dashboard)|
| permissionsApiHostName| (The FQDN of the Permissions API) |
| permissionsApiCertificateSecretValue| (Certificate encoded to base64 string generated in step 5)|
| permissionsApiCertificatePassphraseSecretValue| (Certificate password generated in step 5)|
| saasAppApiScopes| (space delimited string of SaaS App Api scope names (e.g. "test.scope tenant.delete tenant.global.delete tenant.global.read tenant.global.write tenant.read tenant.write"))                         |
| saasProviderName| (created in step 8)                      |
| saasEnvironment| (created in step 8)|
| saasInstanceNumber| (created in step 8)|
| sqlAdministratorLogin| (created in step 8)        |
| sqlAdministratorLoginPassword| (created in step 8) |

#### Example

```
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
	"adminApiScopes": {
      "value": "test.scope tenant.delete tenant.global.delete tenant.global.read tenant.global.write tenant.read tenant.write"
    },
	"adminApiScopeBaseUrl": {
      "value": "https://asdkdemotenant.onmicrosoft.com/00000000-0000-0000-0000-000000000000"
    },
	"azureAdB2cAdminApiClientIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
    "AppSettings:AdminServiceBaseUrl": {
      "value": "asdk-demo-admin.com"
    },
    "azureAdB2cDomainSecretValue": {
      "value": "asdkdemotenant.onmicrosoft.com"
    },
    "azureAdB2cInstanceSecretValue": {
      "value": "https://asdkdemotenant.b2clogin.com"
    },
	"azureAdB2cSaasAppClientIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
	"azureAdB2cSaasAppClientSecretSecretValue": {
      "value": "FooBar"
    },
	"azureAdB2cSignupAdminClientIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
	"azureAdB2cSignupAdminClientSecretSecretValue": {
      "value": "FooBar"
    },
    "azureAdB2cTenantIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
    "azureAdB2cPermissionsApiClientIdSecretValue": {
      "value": "00000000-0000-0000-0000-000000000000"
    },
    "azureAdB2cPermissionsApiClientSecretSecretValue": {
      "value": "FooBar"
    },
    "permissionsApiSslThumbprintSecretValue": {
      "value": "FooBar"
    },
    "permissionsApiHostName": {
      "value": "https://asdkdemopermissions.com"
    },
    "permissionsApiCertificateSecretValue": {
      "value": "FooBar"
    },
    "permissionsApiCertificatePassphraseSecretValue": {
      "value": "FooBar1"
    },
    "saasAppApiScopes": {
      "value": "test.scope tenant.delete tenant.global.delete tenant.global.read tenant.global.write tenant.read tenant.write"
    },
    "saasProviderName": {
      "value": "contoso"
    },
    "saasEnvironment": {
      "value": "tst"
    },
    "saasInstanceNumber": {
      "value": "001"
    },
    "sqlAdministratorLogin": {
      "value": "sqlAdmin"
    },
    "sqlAdministratorLoginPassword": {
      "value": "FooBar1"
    }
  }
}
```
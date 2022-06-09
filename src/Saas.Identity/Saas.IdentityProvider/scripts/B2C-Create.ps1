#### Settings ####
$ErrorActionPreference = "Stop"
#### /Settings ####


# Windows PowerShell and PowerShell Core are supported.
# - Microsoft.Graph PowerShell module needs to be installed.
# - Azure CLI needs to be installed and authenticated for the owning tenant.
#
# Usage:
# - dot-source in a PS script: . ./Create-AzureB2C.ps1
# - invoke individual functions, or the main one: Initialize-B2CTenant -B2CTenantName mytenant -ResourceGroupName myrg -Location "Europe" -CountryCode "CZ"

function New-SaaSIdentityProvider {
  [CmdletBinding()] # indicate that this is advanced function (with additional params automatically added)
  param (
    # [Parameter(Mandatory = $true, HelpMessage = "B2C tenant name, without the '.onmicrosoft.com'.")]
    # [string] $B2CTenantName,
      
    # [Parameter(Mandatory = $true, HelpMessage = "Name of the Azure Resource Group to put the Identity Framework resources into. Will be created if it does not exist.")]
    # [string] $IdentityFrameworkResourceGroupName,

    # # [Parameter(Mandatory = $true, HelpMessage = "Name of the Azure Resource Group to put the application layer resources into. Will be created if it does not exist.")]
    # # [string] $ApplicationLayerResourceGroupName,

    # [Parameter(Mandatory = $true, HelpMessage = "Location of the Azure Resources to be deployed. Run az account list-locations to see the available locations.")]
    # [string] $AzureResourceLocation,
  
    # [string] $B2CTenantLocation = "United States",
      
    # [Parameter(HelpMessage = "Two letter country code (e.g. 'US', 'CZ', 'DE'). https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency")]
    # [string] $CountryCode = "US"
  )
  
  if (Get-Module -ListAvailable -Name Microsoft.Graph) {
    Write-Host "Module Microsoft.Graph exists."
  }
  else {
    throw "Module Microsoft.Graph is not installed yet. Please install it first! Run 'Install-Module Microsoft.Graph'."
  }

  $userInputParams = Get-UserInputParameters
  
  # Create the B2C tenant resource in Azure and capture the Guid of the resource.
  $createdTenantGuid = New-AzureADB2CTenant `
    -B2CTenantName $userInputParams.B2CTenantName `
    -B2CTenantLocation $userInputParams.B2CTenantLocation `
    -CountryCode $userInputParams.CountryCode `
    -AzureResourceLocation $userInputParams.AzureResourceLocation `
    -AzureResourceGroup $userInputParams.IdentityFrameworkResourceGroupName `
  
  # Call the init API
  Invoke-TenantInit `
    -B2CTenantName $userInputParams.B2CTenantName
  
  Write-Host "We must now authenticate against into the Microsoft Graph Service."
  Write-Host "Starting Interactive login to Microsoft Graph. Watch for a newly opened browser window (or device flow instructions) and complete the sign in."
  # Interactive login, so that we don't have to create a separate service principal and handle secrets.
  # Make sure that the user has administrative permissions in the tenant.
  Connect-MgGraph -TenantId "$($userInputParams.B2CTenantName).onmicrosoft.com" -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "Directory.AccessAsUser.All", "Directory.ReadWrite.All", "TrustFrameworkKeySet.ReadWrite.All, Policy.ReadWrite.TrustFramework"
  

  #   return @{
  #     AdminAppReg       = $adminAppReg
  #     SignupAdminAppReg = $signupAdminAppReg
  #     PermissionsAppReg = $permissionsAppReg
  #     SaasAppAppReg     = $saasAppAppReg
  #     IEFAppReg         = $iefAppReg
  #     IEFProxyAppReg    = $iefProxyAppReg
  # }

  # App ID for IEF Proxy App reg $appRegistrations.IEFProxyAppReg.AppRegistrationProperties.AppId

  $appRegistrations = Install-AppRegistrations `
    -B2CTenantName $userInputParams.B2CTenantName `
    -SignupAdminFQDN $userInputParams.SignupAdminFQDN `
    -SaasAppFQDN $userInputParams.SaasAppFQDN `

  $selfSignedCert = New-AsdkSelfSignedCertificate

  # Deploy Bicep here

  Invoke-IdentityBicepDeployment `
    -IdentityFrameworkResourceGroupName $userInputParams.IdentityFrameworkResourceGroupName `
    -B2CDomain "https://$($userInputParams.B2CTenantName).b2clogin.com" `
    -B2CInstanceName "$($userInputParams.B2CTenantName).onmicrosoft.com" `
    -B2cTenantId $createdTenantGuid `
    -PermissionsApiAppRegClientId $appRegistrations.PermissionsAppReg.AppRegistrationProperties.AppId `
    -PermissionsApiAppRegClientSecret $appRegistrations.PermissionsAppReg.ClientSecret `
    -PermissionsApiSelfSignedCertThumbprint $selfSignedCert.PfxThumbprint `
    -AzureAdUserID $userInputParams.UserId `
    -SaasProviderName $userInputParams.ProviderName `
    -SaasEnvironment $userInputParams.SaasEnvironment `
    -SaasInstanceNumber $userInputParams.InstanceNumber `
    -SqlAdministratorLogin $userInputParams.SqlAdministratorLogin `
    -SqlAdministratorPassword $userInputParams.SqlAdministratorLoginPassword `

  
  #Create Signing and Encrpytion Keys
  $trustFrameworkKeySetSigningKeyId = New-TrustFrameworkSigningKey 
  $trustFrameworkKeySetEncryptionKeyId = New-TrustFrameworkEncryptionKey
  
  # Upload cert to b2c here
  $trustFrameworkKeySetClientCertificateKeyId = New-TrustFrameworkClientCertificateKey -Key $selfSignedCert.PfxString -Pswd $selfSignedCert.Pswd

  # Upload policies
  $configTokens = @{
      "{Settings:Tenant}" = "$($userInputParams.B2CTenantName).onmicrosoft.com"
      "{Settings:ProxyIdentityExperienceFrameworkAppId}" = "$($appRegistrations.IEFAppReg.AppRegistrationProperties.AppId)"
      "{Settings:IdentityExperienceFrameworkAppId}" = "$($appRegistrations.IEFProxyAppReg.AppRegistrationProperties.AppId)"
      "{Settings:PermissionsAPIUrl}" = "https://appapplication$($userInputParams.ProviderName)$($userInputParams.SaasEnvironment).azurewebsites.net/api/CustomClaims/permissions"
      "{Settings:RolesAPIUrl}" = "https://appapplication$($userInputParams.ProviderName)$($userInputParams.SaasEnvironment).azurewebsites.net/api/CustomClaims/roles"
      "{Settings:RESTAPIClientCertificate}" = "$($trustFrameworkKeySetClientCertificateKeyId)"  
}

  Import-IEFPolicies -configTokens $configTokens
  # Output parameters.json

}

function Get-UserInputParameters {
 
  $userInputParams = @{
    B2CTenantName = Read-Host "Please enter a name for the B2C tenant without the onmicrosoft.com suffix. (e.g. mytenant). Please note that tenant names must be globally unique."
    B2CTenantLocation = Read-Host "Please enter the location for the B2C Tenant to be created in. (United States', 'Europe', 'Asia Pacific', 'Australia)"
    CountryCode = Read-Host "Please enter the two letter country code for the B2C Tenant data to be stored in (e.g. 'US', 'CZ', 'DE'). See https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency for the list of available country codes."
    AzureResourceLocation = Read-Host "Please enter the location for the Azure Resources to be deployed (e.g. 'eastus', 'westus2', 'centraleurope'). Please run az account list-locations to see the available locations for your account."
    IdentityFrameworkResourceGroupName = Read-Host "Please enter the name of the Azure Resource Group to put the Identity Framework resources into. Will be created if it does not exist."
    SaasEnvironment = Read-Host "Please enter an environment name. Accepted values are: 'prod', 'staging', 'dev', 'test'"
    ProviderName = Read-Host "Please enter a provider name. This name will be used to name the Azure Resources. (e.g. contoso, myapp)"
    InstanceNumber = Read-Host "Please enter an instance number. This number will be appended to most Azure Resources created. (e.g. 001, 002, 003)"
    UserId = az account show --query "id" -o tsv
    SqlAdministratorLogin = Read-Host "Please enter the desired username for the SQL administrator account (e.g. admin)"
    SqlAdministratorLoginPassword = Read-Host -MaskInput -Prompt "Please enter the desired password for the SQL administrator account."
  }

  # $userInputParams = @{
  #   B2CTenantName                      = "lpb2ctest02"
  #   B2CTenantLocation                  = "United States"
  #   CountryCode                        = "US"
  #   AzureResourceLocation              = "eastus"
  #   IdentityFrameworkResourceGroupName = "rg-identity-02"
  #   SaasEnvironment                    = "dev"
  #   ProviderName                       = "lbptst"
  #   InstanceNumber                     = "004"
  #   UserId                             = az account show --query "id" -o tsv
  #   SqlAdministratorLogin              = "lpadmin"
  #   SqlAdministratorLoginPassword      = Read-Host -MaskInput -Prompt "Please enter the desired password for the SQL administrator account." #  "asJ1@mf#!aks*"
  # }

  $userInputParams.Add("SaasAppFQDN", "https://appapplication$($userInputParams.ProviderName)$($userInputParams.SaasEnvironment).azurewebsites.net")
  $userInputParams.Add("SignupAdminFQDN", "https://appsignup$($userInputParams.ProviderName)$($userInputParams.SaasEnvironment).azurewebsites.net")
  $userInputParams.Add("PermissionsApiFQDN", "https://apipermissions$($userInputParams.ProviderName)$($userInputParams.SaasEnvironment).azurewebsites.net")

  return $userInputParams

}

function New-AzureADB2CTenant {
  param(
    # Tenant name without the '.onmicrosoft.com' part.
    [string] $B2CTenantName,
  
    # Can be one of 'United States', 'Europe', 'Asia Pacific', or 'Australia' (preview).
    [Parameter()]
    [ValidateSet('United States', 'Europe', 'Asia Pacific', 'Australia')]
    [string] $B2CTenantLocation,

    [Parameter(Mandatory = $true, HelpMessage = "Location of the Azure Resources to be deployed. Run az account list-locations to see the available locations.")]
    [string] $AzureResourceLocation,
  
    # Where data resides. Two letter country code (e.g. 'US', 'CZ', 'DE').
    # Valid country codes are listed here: https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency
    [string] $CountryCode,
  
    # Under which Azure subscription will this B2C tenant reside. If not provided, use the current subscription from Azure CLI.
    [string] $AzureSubscriptionId = $null,
  
    # Under which Azure resource group will this B2C tenant reside.
    [string] $AzureResourceGroup

  )
  
  if (!$AzureSubscriptionId) {
    Write-Host "Getting subscription ID from the current account..."
    $AzureSubscriptionId = $(az account show --query "id" -o tsv)
    Write-Host "The default subscription ID from the current account is ${AzureSubscriptionId}"
    $UseDefaultSubscriptionId = Read-Host -Prompt "Is this the subscription you'd like to use? (y/n) "

    if ($UseDefaultSubscriptionId -eq "n") {
      $AzureSubscriptionId = Read-Host -Prompt "Please provide the subscription ID of the subscription you'd like to use"
    }

    Write-Host "Using subscription ID ${AzureSubscriptionID}"
    az account set --subscription $AzureSubscriptionId
  }
  
  $aadProviderRegState = $(az provider show -n Microsoft.AzureActiveDirectory --query "registrationState" -o tsv)
  if ($aadProviderRegState -ne "Registered") {
    Write-Host "Resource Provider 'Microsoft.AzureActiveDirectory' not registered yet. Registering now..."
    az provider register --namespace Microsoft.AzureActiveDirectory
  
    while ($(az provider show -n Microsoft.AzureActiveDirectory --query "registrationState" -o tsv) -ne "Registered") {
      Write-Host "Resource Provider registration not yet finished. Waiting..."
      Start-Sleep -Seconds 10
    }
    Write-Host "Resource Provider registration finished."
  }
  
  Write-Host "Checking if Resource Group $AzureResourceGroup exists..."
  $checkRg = az group exists --name $AzureResourceGroup | ConvertFrom-Json
  
  if ($LastExitCode -ne 0) {
    throw "Error on using Azure CLI. Make sure the CLI is installed, up-to-date and you are signed in. Run 'az login' to sign in."
  }
  
  if (!$checkRg) {
    Write-Warning "Resource Group $AzureResourceGroup does not exist. Creating..."
    az group create --name $AzureResourceGroup --location $AzureResourceLocation

    do {
      Write-Host "Waiting for 15 seconds for Resource Group creation..."
      Start-Sleep -Seconds 15
  
      az group show --name $AzureResourceGroup
    }
    while ($LastExitCode -ne 0)
  
    Write-Host "Resource Group Created Successfuly"
  }

  else {
    Write-Host "Resource Group $AzureResourceGroup already exists. Skipping creation."
  }
  
  $resourceId = "/subscriptions/$AzureSubscriptionId/resourceGroups/$AzureResourceGroup/providers/Microsoft.AzureActiveDirectory/b2cDirectories/$B2CTenantName.onmicrosoft.com"
  
  # Check if tenant already exists
  Write-Host "Checking if tenant '$B2CTenantName' already exists..."
  az resource show --id $resourceId | Out-Null
  if ($LastExitCode -eq 0) {
    # No error means, the resource exists
    Write-Warning "Tenant '$B2CTenantName' already exists. Not attempting to recreate it."
    return $(az resource show --id $resourceId --query "properties.tenantId" -o tsv)
  }
  
  $reqBody =
  @"
    {
      "location":"$($B2CTenantLocation)",
      "sku": {
          "name":"PremiumP1",
          "tier":"A0"
      },
      "properties": {
          "createTenantProperties": {
              "displayName":"$($B2CTenantName)",
              "countryCode":"$($CountryCode)"
          }
      }
    }
"@


  # Flatten the JSON to make Azure CLI happy, otherwise it complains about incorrect content type.
  $reqBody = $reqBody.Replace("`n", "").Replace("`"", "\`"")

  Write-Host "Creating B2C tenant $B2CTenantName..."
  # https://docs.microsoft.com/en-us/rest/api/activedirectory/b2c-tenants/create
  az rest --method PUT --uri "https://management.azure.com$($resourceId)?api-version=2019-01-01-preview" --body $reqBody

  if ($LastExitCode -ne 0) {
    throw "Error on creating new B2C tenant!"
  }

  Write-Host "*** B2C Tenant creation started. It can take a moment to complete."

  do {
    Write-Host "Waiting for 30 seconds for B2C tenant creation..."
    Start-Sleep -Seconds 30

    az resource show --id $resourceId
  }
  while ($LastExitCode -ne 0)
  $tenantGuid = $(az resource show --id $resourceId --query "properties.tenantId" -o tsv)
  return $tenantGuid
}

  
#
# Finalize initialization of newly created B2C tenant.
# This function needs to be called once the tenant is created and before any other steps, because it creates the b2c-extensions-app.
#
# Required: Azure CLI authenticated with owner permissions for the tenant.
function Invoke-TenantInit {
  param (
    [string] $B2CTenantName
  )
  
  $B2CTenantId = "$($B2CTenantName).onmicrosoft.com"
  
  # Get access token for the B2C tenant with audience "management.core.windows.net".
  $managementAccessToken = $(az account get-access-token --tenant "$($B2CTenantId)" --query accessToken -o tsv)
  
  # Invoke tenant initialization which happens through the portal automatically.
  # Ref: https://stackoverflow.com/questions/67706798/creation-of-the-b2c-extensions-app-by-script
  Write-Host "Invoking tenant initialization..."
  Invoke-WebRequest -Uri "https://main.b2cadmin.ext.azure.com/api/tenants/GetAndInitializeTenantPolicy?tenantId=$($B2CTenantId)&skipInitialization=false" `
    -Method "GET" `
    -Headers @{
    "Authorization" = "Bearer $($managementAccessToken)"
  }
}


function New-TrustFrameworkSigningKey {
  Write-Host "Creating new signing key..."
  $trustFrameworkKeySetName = "TokenSigningKeyContainer"
  $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
  New-MgTrustFrameworkKeySetKey -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -Kty "RSA" -Use "Sig"
  return $trustFrameworkKeySetName
}

function New-TrustFrameworkEncryptionKey {
  Write-Host "Creating new encryption key..."
  $trustFrameworkKeySetName = "TokenEncryptionKeyContainer"
  $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
  New-MgTrustFrameworkKeySetKey -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -Kty "RSA" -Use "Enc"
  return $trustFrameworkKeySetName
}

function New-TrustFrameworkClientCertificateKey {
  param (
    [string] $Key = "",
    [Security.SecureString] $Pswd
  )
  Write-Host "Creating client certificate policy..."
  $trustFrameworkKeySetName = "RestApiClientCertificate"
  $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
  $params = @{
    Key      = $Key
    Password = ConvertFrom-SecureString -SecureString $Pswd -AsPlainText
  }
  Invoke-MgUploadTrustFrameworkKeySetPkcs12 -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -BodyParameter $params
  return $trustFrameworkKeySetName
}

function Import-IefPolicies {
  param (
    [string] $IEFPoliciesSourceDirectory = "../policies",
    [hashtable] $configTokens
  )
  Write-Host "Importing IEF policies..."
 
  #get XML Files in directory
  $policyFiles = Get-Childitem -Path $IEFPoliciesSourceDirectory -Filter '*.xml'
  $customPolicyList = @()
  
  try {
    #Replace tokens in each file
    Write-Host "Replacing tokens in IEF policies..."
    foreach ($file in $policyFiles) {
      $policy = Get-Content -Path $file.FullName
        
      #replace config tokens
      Write-Host "Replacing tokens in $($file.Name)"
      $configTokens.GetEnumerator() | ForEach-Object {
        $policy = $policy.Replace($_.Key, $_.Value)
      }
      [xml]$policyXml = $policy

      #Get Policy information
      $policy = [PSCustomObject]@{
        "Id"           = $policyXml.TrustFrameworkPolicy.PolicyId
        "BasePolicyId" = If ([string]::IsNullOrWhitespace($policyXml.TrustFrameworkPolicy.BasePolicy.PolicyId)) { $null } Else { $policyXml.TrustFrameworkPolicy.BasePolicy.PolicyId }
        "Xml"          = $policyXml
      }
      $customPolicyList += $policy
    } 
    #upload files
    #TODO: Handle errors when uploading files
    #TODO: Handle if file already exists
    Write-Host $customPolicyList
    $basePolicy = $customPolicyList | Where-Object { $null -eq $_.BasePolicyId }  | Select-Object -First 1
    Write-Host "Uploading base policy $($basePolicy.Id)..."
    Invoke-MgGraphRequest -Uri "https://graph.microsoft.com/beta/trustFramework/policies" -Body $basePolicy[0].Xml.OuterXml -ContentType "application/xml" -Method "POST"
    Import-ChildTrustFrameworkPolicies -CustomPolicyList $customPolicyList  -PolicyId $($basePolicy.Id)

  }
  catch {
    Write-Host "An error occurred: $_.Exception.Message"
  }
}
 
function Import-ChildTrustFrameworkPolicies {
  param (
    [object[]] $CustomPolicyList,
    [string] $PolicyId
  )
  Write-Host "Base Policy is $($PolicyId)"
  $customPolicyList | Where-Object { $_.BasePolicyId -eq $PolicyId }  | ForEach-Object {
    Write-Host "Uploading policy $($_.Id)..."
    Invoke-MgGraphRequest -Uri "https://graph.microsoft.com/beta/trustFramework/policies" -Body $_.Xml.OuterXml -ContentType "application/xml" -Method "POST"
    Import-ChildTrustFrameworkPolicies -customPolicyList $customPolicyList -PolicyId $_.Id
  }

}
#Executes the Bicep template to install
function Invoke-IdentityBicepDeployment {
  param (
    [string] $IdentityFrameworkResourceGroupName,
    [string] $BicepTemplatePath = "../../Saas.Identity.IaC/main.bicep",
    [string] $B2CDomain,
    [string] $B2CInstanceName,
    [string] $B2cTenantId,
    [string] $PermissionsApiAppRegClientId,
    [string] $PermissionsApiAppRegClientSecret,
    [string] $PermissionsApiSelfSignedCertThumbprint,
    [string] $AzureAdUserID,
    [string] $SaasProviderName,
    [string] $SaasEnvironment,
    [string] $SaasInstanceNumber,
    [string] $SqlAdministratorLogin,
    [string] $SqlAdministratorPassword
    
  )

  $params = @{
    azureAdB2cDomainSecretValue                     = $B2CDomain
    azureAdB2cInstanceSecretValue                   = $B2CInstanceName
    azureAdB2cTenantIdSecretValue                   = $B2cTenantId
    azureAdB2cPermissionsApiClientIdSecretValue     = $PermissionsApiAppRegClientId
    azureAdB2cPermissionsApiClientSecretSecretValue = $PermissionsApiAppRegClientSecret
    permissionsApiSslThumbprintSecretValue          = $PermissionsApiSelfSignedCertThumbprint
    azureAdUserID                                   = $AzureAdUserID
    saasProviderName                                = $SaasProviderName
    saasEnvironment                                 = $SaasEnvironment
    saasInstanceNumber                              = $SaasInstanceNumber
    sqlAdministratorLogin                           = $SqlAdministratorLogin
    sqlAdministratorLoginPassword                   = $SqlAdministratorPassword
  } 

  $convertedParams = ConvertTo-AzJsonParams -params $params
  $paramsJson = (( $convertedParams | ConvertTo-Json -Compress) -replace '([\\]*)"', '$1$1\"')

  Write-Host "Beginning Identity Provider Bicep Deployment...."

  az deployment group create `
    --name "IdentityBicepDeployment" `
    --resource-group "$IdentityFrameworkResourceGroupName" `
    --template-file "$BicepTemplatePath" `
    --parameters $paramsJson

}

# TODO: This is currently windows specific. Need to make this cross platform.
function New-AsdkSelfSignedCertificate {
  try{
    Write-Host "Creating self signed certificate..."
    $selfSignedCert = New-SelfSignedCertificate -CertStoreLocation "Cert:\CurrentUser\My" -DnsName *.azurewebsites.net -NotAfter (Get-Date).AddYears(2)

    $pswd = ConvertTo-SecureString -String "1234" -Force -AsPlainText
    Export-PfxCertificate -cert $selfSignedCert.PSPath -FilePath "selfSignedCertificate.pfx" -Password $pswd

    $pfxThumbprint = $selfSignedCert.Thumbprint
    $pfxBytes = Get-Content "selfSignedCertificate.pfx" -AsByteStream
    $pfxString = [System.Convert]::ToBase64String($pfxBytes)

  }
  catch {
    Write-Host "An error occurred: $_.Exception.Message"
  }
  
  return @{ PfxString = $pfxString; PfxThumbprint = $pfxThumbprint; Pswd = $pswd }
}

# Helper Function called by Install-AppRegistrations
function New-AppRegistration {
  param (
    [Parameter(Mandatory = $true, HelpMessage = "A Hash table of data to be added to be sent with the app registration creation request.")]
    [hashtable] $AppRegistrationData,

    [Parameter(Mandatory = $false, HelpMessage = "Indicates whether to create a secret for the app registration")]
    [bool] $CreateSecret = $false
  )
  Write-Host "Creating App Registration $($AppRegistrationData.DisplayName)"
  # Create the app registration using the Microsoft Graph API and store the result. 
  $newApp = New-MgApplication `
    -DisplayName $AppRegistrationData.DisplayName `
    -Api @{Oauth2PermissionScopes = $AppRegistrationData.OAuth2PermissionScopes } `
    -IdentifierUris $AppRegistrationData.IdentifierUris `
    -RequiredResourceAccess $AppRegistrationData.RequiredResourceAccess `
    -PublicClient $AppRegistrationData.PublicClient `
    -IsFallbackPublicClient:$AppRegistrationData.IsFallbackPublicClient `
    -Web $AppRegistrationData.Web `
    -AppRoles $AppRegistrationData.AppRoles `

  $newAppSecret = $null
  if ($CreateSecret) {
    $newAppSecretObject = Add-MgApplicationPassword -ApplicationId $newApp.Id 
    $newAppSecret = $newAppSecretObject.SecretText
  }

  # Also need to create the service principal for the app
  Write-Host "Creating Service Principal for App Registration $($AppRegistrationData.DisplayName)"
  $sp = New-MgServicePrincipal -AppId $newApp.AppId -DisplayName $newApp.DisplayName
  Write-Host "Created Service Principal for App Registration $($AppRegistrationData.DisplayName)"

  Write-Host "App Registration $($newApp.DisplayName) Created"
  return @{
    ClientSecret               = $newAppSecret
    AppRegistrationProperties  = $newApp
    ServicePrincipalProperties = $sp
  }
}

# Helper Function called by Install-AppRegistrations
function New-AdminConsent {
  param(
    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the application that consent is being granted on.")]
    [string] $ClientObjectId,

    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the API application for which consent is being granted for.")]
    [string] $ApiObjectId,

    [Parameter(Mandatory = $true, HelpMessage = "The Scopes for which consent is being granted.")]
    [array] $ApiScopes,

    [Parameter(Mandatory = $false, HelpMessage = "The app roles to assign to the application.")]
    [array] $AppRoles = $null
  )

  Write-Host "Granting consent for $ApiScopes on $ClientObjectId for $ApiObjectId"

  if ($AppRoles -ne $null) {
    foreach ($role in $AppRoles) {
      $params = @{
        PrincipalId = $role.PrincipalId
        ResourceId  = $role.ResourceId
        AppRoleId   = $role.AppRoleId 
      }
      New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $role.PrincipalId -BodyParameter $params
      Write-Host "Assigned $($role.AppRoleId) to $($role.PrincipalId)"
    }
  }
  
  $payload = @{
    ConsentType = "AllPrincipals"
    ClientId    = $ClientObjectId 
    ResourceId  = $ApiObjectId
    Scope       = $ApiScopes -Join " " #"tenant.delete tenant.write tenant.global.delete tenant.global.write tenant.read tenant.global.read"
  }
  try {
    New-MgOauth2PermissionGrant -BodyParameter $payload 
    Write-Host "Consent granted"
  }
  catch {
    Write-Error "Error granting admin consent: $_"
    throw
  }

  
  
}

# Usage:
# Install-AppRegistrations -TenantId $TenantId -SignupAdminFQDN $SignupAdminFQDN -SaasAppFQDN $SaasAppFQDN

# Required Modules:
# - Microsoft.Graph
# - Microsoft.Graph.Applications
# - Microsoft.Graph.Identity.SignIns

# Output: 
# Hashtable with the following properties:
#       AdminAppReg, Hashtable see schema below
#       SignupAdminAppReg, Hashtable see schema below
#       PermissionsAppReg, Hashtable see schema below
#       SaasAppAppReg, Hashtable see schema below
#       IEFAppReg, Hashtable see schema below
#       IEFProxyAppReg, Hashtable see schema below

# Each hashtable property is a hashtable with the following properties:
# ClientSecret - String, The client secret for the app registration, if created
# AppRegistrationProperties - Hashtable, Schema of app reg properties found here: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.powershell.cmdlets.resources.msgraph.models.apiv10.imicrosoftgraphapplication?view=az-ps-latest
# ServicePrincipalProperties - Hashtable, Schema of Service principal properties found here: https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.powershell.cmdlets.resources.msgraph.models.apiv10.microsoftgraphserviceprincipal?view=az-ps-latest
function Install-AppRegistrations {
  param(
    [Parameter(Mandatory = $true, HelpMessage = "The Tenant Identifier")]
    [string] $B2CTenantName,
    [Parameter(Mandatory = $true, HelpMessage = "The estimated FQDN for the signupadmin azure app service")]
    [string] $SignupAdminFQDN,
    [Parameter(Mandatory = $true, HelpMessage = "The estimated FQDN for the saas app azure app service")]
    [string] $SaasAppFQDN
  )

  Write-Host "Beginning Creating App Registrations"
  $msGraphAccess = @{
              
    ResourceAppId  = "00000003-0000-0000-c000-000000000000"
    ResourceAccess = @(
      @{
        Id   = "37f7f235-527c-4136-accd-4a02d197296e"
        Type = "Scope"
      },
      @{
        Id   = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182"
        Type = "Scope"
      }
    )
  }

  # Gets the ms graph service principal for this account.
  $msGraphServicePrincipal = Get-MgServicePrincipal -Filter "appId eq '00000003-0000-0000-c000-000000000000'" -CountVariable CountVar -Top 1 -ConsistencyLevel eventual

  ############## Admin API App Registration ##############

  $adminAppRegConfig = @{
    DisplayName            = "asdk-admin-api"
    IdentifierUris         = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @(
      @{
        AdminConsentDisplayName = "Allows deletion of tenants";
        AdminConsentDescription = "Allows deletion of tenants";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.global.delete";
      },
      @{
        AdminConsentDisplayName = "Allow deletion of user's tenants";
        AdminConsentDescription = "Allow deletion of user's tenants";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.delete";
      },
      @{
        AdminConsentDisplayName = "Write to user's tenants";
        AdminConsentDescription = "Write to user's tenants";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.write";
      },
      @{
        AdminConsentDisplayName = "Write to all tenants";
        AdminConsentDescription = "Write to all tenants";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.global.write";
      },
      @{
        AdminConsentDisplayName = "Allow reading of tenant data across all tenants";
        AdminConsentDescription = "Allow reading of tenant data across all tenants";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.global.read";
      },
      @{
        AdminConsentDisplayName = "Allow reading tenant data for current user";
        AdminConsentDescription = "Allow reading tenant data for current user";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "tenant.read";
      }
    )
    RequiredResourceAccess = @($msGraphAccess)
    IsFallbackPublicClient = $false 
    PublicClient           = @{}
    Web                    = @{ 
      ImplicitGrantSettings = @{
        EnableAccessTokenIssuance = $true
        EnableIdTokenIssuance     = $true
      }
      LogoutUrl             = ""
      RedirectUris          = @()
    }
    AppRoles               = @{}
  }

  # Create the App Registration
  $adminAppReg = New-AppRegistration -AppRegistrationData $adminAppRegConfig



  ############## Signup Admin App Registration ##############


  $signupAdminAppRegConfig = @{
    DisplayName            = "asdk-signupadmin-app"
    IdentifierUri          = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @()
    RequiredResourceAccess = @(@{
        ResourceAppId  = $adminAppReg.AppRegistrationProperties.AppId
        ResourceAccess = $adminAppRegConfig.OAuth2PermissionScopes | ForEach-Object { @{Id = $_.Id; Type = "Scope" } }
      },
      $msGraphAccess # Add Default Microsoft Graph permissions
    )
  
    IsFallbackPublicClient = $false
    PublicClient           = @{ redirectUris = @("$($SaasAppFQDN)/signin-oidc") }
    Web                    = @{ 
      ImplicitGrantSettings = @{
        EnableAccessTokenIssuance = $true
        EnableIdTokenIssuance     = $true
      }
      LogoutUrl             = "$($SignupAdminFQDN)/signout-oidc"
      RedirectUris          = @("$($SignupAdminFQDN)/signin-oidc")
    }
    AppRoles               = @{}

  }

  # Create the App Registration
  $signupAdminAppReg = New-AppRegistration -AppRegistrationData $signupAdminAppRegConfig -CreateSecret $true
  # Get the scopes from the admin app registration
  $adminScopes = $adminAppRegConfig.OAuth2PermissionScopes | ForEach-Object { $_.Value }
  # Grant admin consent on the signupadmin app for the admin scopes
  New-AdminConsent -ClientObjectId $signupAdminAppReg.ServicePrincipalProperties.Id -ApiObjectId $adminAppReg.ServicePrincipalProperties.Id -ApiScopes $adminScopes

  ############## Permissions API Registration ##############

  $permissionsAppRegConfig = @{
    DisplayName            = "asdk-permissions-api"
    IdentifierUri          = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @()
    RequiredResourceAccess = @(@{
        ResourceAppId  = $msGraphAccess.ResourceAppId
        ResourceAccess = @(
          @{
            ID   = "df021288-bdef-4463-88db-98f22de89214"
            Type = "Role"
          },
          @{
            ID   = "9a5d68dd-52b0-4cc2-bd40-abcf44ac3a30"
            Type = "Role"
          },
          $msGraphAccess.ResourceAccess[0], #Add default graph access in addtion to the permissions the permissions api needs as well. 
          $msGraphAccess.ResourceAccess[1]
        )
      }
    )
    IsFallbackPublicClient = $false
    PublicClient           = @{}
    Web                    = @{}
    AppRoles               = @{


    }

  }

  # Create the App Registration
  $permissionsAppReg = New-AppRegistration -AppRegistrationData $permissionsAppRegConfig -CreateSecret $true
  $permissionsAppRegGraphAppRoles = @( # App Roles to add to the permissions API. Namely the two for MS graph (Application.Read.All and User.Read.All)
    @{
      PrincipalId = $permissionsAppReg.ServicePrincipalProperties.Id
      ResourceId  = $msGraphServicePrincipal.Id # MS Graph Resource ID
      AppRoleId   = "df021288-bdef-4463-88db-98f22de89214" # App Role ID for Application.Read.All
    },
    @{
      PrincipalId = $permissionsAppReg.ServicePrincipalProperties.Id
      ResourceId  = $msGraphServicePrincipal.Id # MS Graph Resource ID
      AppRoleId   = "9a5d68dd-52b0-4cc2-bd40-abcf44ac3a30" # App Role ID for User.Read.All
    }
  )
  # Grant Admin consent for the scopes and app roles to MS Graph API
  New-AdminConsent -ClientObjectId $permissionsAppReg.ServicePrincipalProperties.Id `
    -ApiObjectId "00000003-0000-0000-c000-000000000000" `
    -ApiScopes @("Application.Read.All", "offline_access", "openid", "User.Read.All") `
    -AppRoles $permissionsAppRegGraphAppRoles `

  ############## SaaS App App Registration ##############

  $saasAppAppRegConfig = @{
    DisplayName            = "asdk-saas-app"
    IdentifierUri          = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @()
    RequiredResourceAccess = @(@{
        ResourceAppId  = $adminAppReg.AppRegistrationProperties.AppId
        ResourceAccess = @($adminAppRegConfig.OAuth2PermissionScopes | Where-Object { $_.Value -eq "tenant.read" } | ForEach-Object { @{Id = $_.Id; Type = "Scope" } })
      },
      $msGraphAccess # Add Default Microsoft Graph permissions
    )
    IsFallbackPublicClient = $false
    PublicClient           = @{ redirectUris = @("$($SaasAppFQDN)/signin-oidc") }
    Web                    = @{ 
      ImplicitGrantSettings = @{
        EnableAccessTokenIssuance = $true
        EnableIdTokenIssuance     = $true
      }
      LogoutUrl             = "$($SaasAppFQDN)/signout-oidc"
      RedirectUris          = @("$($SaasAppFQDN)/signin-oidc")
    }
    AppRoles               = @{}

  }

  # Create the App Registration
  $saasAppAppReg = New-AppRegistration -AppRegistrationData $saasAppAppRegConfig -CreateSecret $true
  # Get the scopes from the admin app registration
  $adminScopesForSaasApp = @($adminAppRegConfig.OAuth2PermissionScopes | Where-Object { $_.Value -eq "tenant.read" } | ForEach-Object { $_.Value })
  # Grant admin consent on the saas app app for the admin scopes
  New-AdminConsent -ClientObjectId $saasAppAppReg.ServicePrincipalProperties.Id -ApiObjectId $adminAppReg.ServicePrincipalProperties.Id -ApiScopes $adminScopesForSaasApp

  
  ############# Create the IEF App Registration ##############

  $iefAppRegConfig = @{
    DisplayName            = "IdentityExperienceFramework"
    IdentifierUris         = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @(
      @{
        AdminConsentDisplayName = "Access IdentityExperienceFramework";
        AdminConsentDescription = "Allow the application to access IdentityExperienceFramework on behalf of the signed-in user.";
        Id                      = New-Guid;
        Type                    = "Admin";
        Value                   = "user_impersonation";
      }

    )
    RequiredResourceAccess = @($msGraphAccess)
    IsFallbackPublicClient = $false
    PublicClient           = @{}
    Web                    = @{ 
      ImplicitGrantSettings = @{
        EnableAccessTokenIssuance = $false
        EnableIdTokenIssuance     = $false
      }
      LogoutUrl             = ""
      RedirectUris          = @("https://$B2CTenantName.b2clogin.com/$B2CTenantName.onmicrosoft.com")
    }
    AppRoles               = @{}
  }

  # Create the App Registration
  $iefAppReg = New-AppRegistration -AppRegistrationData $iefAppRegConfig


  
  ############# Create the IEF Proxy App Registration ##############


  $iefProxyAppRegConfig = @{
    DisplayName            = "ProxyIdentityExperienceFramework"
    IdentifierUri          = @("https://$($B2CTenantName).onmicrosoft.com/$(New-Guid)")
    OAuth2PermissionScopes = @()
    RequiredResourceAccess = @(@{
        ResourceAppId  = $iefAppReg.AppRegistrationProperties.AppId
        ResourceAccess = @($iefAppRegConfig.OAuth2PermissionScopes | ForEach-Object { @{Id = $_.Id; Type = "Scope" } })
      },
      $msGraphAccess # Add Default Microsoft Graph permissions
    )
    IsFallbackPublicClient = $true
    PublicClient           = @{
      redirectUris = @("myapp://auth") 
    }
    Web                    = @{ 
      ImplicitGrantSettings = @{ }
      LogoutUrl             = ""
      RedirectUris          = @()
    }
    AppRoles               = @{}

  }

  # Create the App Registration
  $iefProxyAppReg = New-AppRegistration -AppRegistrationData $iefProxyAppRegConfig
  # Get the scopes from the ief app registration
  $iefScopes = $iefAppRegConfig.OAuth2PermissionScopes | ForEach-Object { $_.Value }
  # Grant admin consent on the ief proxy app for the  ief  scopes
  New-AdminConsent -ClientObjectId $iefProxyAppReg.ServicePrincipalProperties.Id -ApiObjectId $iefAppReg.ServicePrincipalProperties.Id -ApiScopes $iefScopes


  Write-Host "App Registrations Created"
  
  return @{
    AdminAppReg       = $adminAppReg
    SignupAdminAppReg = $signupAdminAppReg
    PermissionsAppReg = $permissionsAppReg
    SaasAppAppReg     = $saasAppAppReg
    IEFAppReg         = $iefAppReg
    IEFProxyAppReg    = $iefProxyAppReg
  }
}

function ConvertTo-AzJsonParams {
  param(
    [Parameter(Mandatory = $true)]
    [hashtable] $params
  )
  $retHashTable = @{}

  foreach ($i in $params.GetEnumerator()) {
    $retHashTable.Add($i.Name, @{value = $i.Value })
  }
  return $retHashTable

}


# Outputs parameters.json file with the information from the b2c setup. 
function Write-OutputFile {
    
}

$loggedIn = az account list | ConvertFrom-Json

if ($loggedIn.Count -eq 0) {
  Write-Error "No Azure account is logged in. Initiating az login command"
  az login
}

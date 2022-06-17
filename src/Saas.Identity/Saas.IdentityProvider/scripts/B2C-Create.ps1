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
  )
  
  if (Get-Module -ListAvailable -Name Microsoft.Graph) {
    Write-Host "Module Microsoft.Graph exists."
  }
  else {
    throw "Module Microsoft.Graph is not installed yet. Please install it first! Run 'Install-Module Microsoft.Graph'."
  }

  Write-Host "Changing Graph Powershell SDK to Beta..."
  Select-MgProfile -Name "beta"

  $userSettings = Invoke-Login
  $userInputParams = Get-UserInputParameters
  
  #get current signed in user
  $adSignedInUser = az account show --query "user.name" -o tsv

  # Create the B2C tenant resource in Azure and capture the Guid of the resource.
  $createdTenantGuid = New-AzureADB2CTenant `
    -B2CTenantName $userInputParams.B2CTenantName `
    -B2CTenantLocation $userInputParams.B2CTenantLocation `
    -CountryCode $userInputParams.CountryCode `
    -AzureSubscriptionId $userSettings.SubscriptionId `
    -AzureResourceLocation $userInputParams.AzureResourceLocation `
    -AzureResourceGroup $userInputParams.IdentityFrameworkResourceGroupName `
  
  Write-Host "We must now authenticate against the Microsoft Graph Service on your newly created tenant."
  Write-Host "Starting Interactive login to Microsoft Graph. Watch for a newly opened browser window (or device flow instructions) and complete the sign in."
  # Interactive login, so that we don't have to create a separate service principal and handle secrets.
  # Make sure that the user has administrative permissions in the tenant.
  Connect-MgGraph -TenantId "$($userInputParams.B2CTenantName).onmicrosoft.com" -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "Directory.AccessAsUser.All", "Directory.ReadWrite.All", "TrustFrameworkKeySet.ReadWrite.All, Policy.ReadWrite.TrustFramework"

  $CurrentB2CUserPrincipalName = $adSignedInUser.Replace('@', '_')
  $currentB2CUser = Get-MgUser -ConsistencyLevel eventual -Count userCount -Filter "startsWith(UserPrincipalName, '$($CurrentB2CUserPrincipalName)')" -Top 1

  $appRegistrations = Install-AppRegistrations `
    -B2CTenantName $userInputParams.B2CTenantName `
    -SignupAdminFQDN $userInputParams.SignupAdminFQDN `
    -SaasAppFQDN $userInputParams.SaasAppFQDN `
    -CurrentB2CUserId $currentB2CUser.Id `

  $selfSignedCert = New-AsdkSelfSignedCertificate $userInputParams.SelfSignedCertificatePassword

  # Deploy Bicep here

  Invoke-IdentityBicepDeployment `
    -IdentityFrameworkResourceGroupName $userInputParams.IdentityFrameworkResourceGroupName `
    -B2CDomain "$($userInputParams.B2CTenantName).onmicrosoft.com" `
    -B2CInstanceName "https://$($userInputParams.B2CTenantName).b2clogin.com" `
    -B2cTenantId $createdTenantGuid `
    -PermissionsApiAppRegClientId $appRegistrations.PermissionsAppReg.AppRegistrationProperties.AppId `
    -PermissionsApiAppRegClientSecret $appRegistrations.PermissionsAppReg.ClientSecret `
    -PermissionsApiSelfSignedCertThumbprint $selfSignedCert.PfxThumbprint `
    -SaasProviderName $userInputParams.ProviderName `
    -SaasEnvironment $userInputParams.SaasEnvironment `
    -SaasInstanceNumber $userInputParams.InstanceNumber `
    -SqlAdministratorLogin $userInputParams.SqlAdministratorLogin `
    -SqlAdministratorPassword $userInputParams.SqlAdministratorLoginPassword `

  
  #Create Signing and Encrpytion Keys
  $trustFrameworkKeySetSigningKeyId = New-TrustFrameworkSigningKey 
  $trustFrameworkKeySetEncryptionKeyId = New-TrustFrameworkEncryptionKey
  
  # Upload cert to b2c here
  $trustFrameworkKeySetClientCertificateKeyId = New-TrustFrameworkClientCertificateKey -CertificateString $selfSignedCert.PfxString -Pswd $userInputParams.SelfSignedCertificatePassword

  # Upload policies
  $configTokens = @{
    "{Settings:Tenant}"                                = "$($userInputParams.B2CTenantName).onmicrosoft.com"
    "{Settings:ProxyIdentityExperienceFrameworkAppId}" = "$($appRegistrations.IEFAppReg.AppRegistrationProperties.AppId)"
    "{Settings:IdentityExperienceFrameworkAppId}"      = "$($appRegistrations.IEFProxyAppReg.AppRegistrationProperties.AppId)"
    "{Settings:PermissionsAPIUrl}"                     = "$($userInputParams.PermissionsApiFQDN)/api/CustomClaims/permissions"
    "{Settings:RolesAPIUrl}"                           = "$($userInputParams.PermissionsApiFQDN)/api/CustomClaims/roles"
    "{Settings:RESTAPIClientCertificate}"              = "$($trustFrameworkKeySetClientCertificateKeyId.Id)"  
  }

  Import-IEFPolicies -configTokens $configTokens

  
    
  # Output parameters.json
  $outputParams = [ordered]@{
    '$schema' = "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"
    contentVersion = "1.0.0.0"
    parameters = [ordered]@{
      adminApiScopes = @{ value = $appRegistrations.AdminAppReg.AppRegistrationConfig.Oauth2PermissionScopes | Join-String -Property Value -Separator " " }
      adminApiScopeBaseUrl = @{ value = $appRegistrations.AdminAppReg.AppRegistrationProperties.IdentifierUris[0] }
      azureAdB2cAdminApiClientIdSecretValue = @{ value = $appRegistrations.AdminAppReg.AppRegistrationProperties.AppId }
      azureAdB2cDomainSecretValue = @{ value = "$($userInputParams.B2CTenantName).onmicrosoft.com" }
      azureAdB2cInstanceSecretValue = @{ value = "https://$($userInputParams.B2CTenantName).b2clogin.com" }
      azureAdB2cSignupAdminClientIdSecretValue = @{ value = $appRegistrations.SignupAdminAppReg.AppRegistrationProperties.AppId }
      azureAdB2cSignupAdminClientSecretSecretValue = @{ value = $appRegistrations.SignupAdminAppReg.ClientSecret }
      azureAdB2cTenantIdSecretValue = @{ value = $createdTenantGuid }
      permissionsApiHostName = @{ value = $userInputParams.PermissionsApiFQDN }
      permissionsApiCertificateSecretValue = @{ value = $selfSignedCert.PfxString }
      permissionsApiCertificatePassphraseSecretValue = @{ value = $userInputParams.SelfSignedCertificatePassword }
      saasProviderName = @{ value = $userInputParams.ProviderName }
      saasEnvironment = @{ value = $userInputParams.SaasEnvironment }
      saasInstanceNumber = @{ value = $userInputParams.InstanceNumber }
      sqlAdministratorLogin = @{ value = $userInputParams.SqlAdministratorLogin }
      sqlAdministratorLoginPassword = @{ value = ConvertFrom-SecureString -SecureString $userInputParams.SqlAdministratorLoginPassword -AsPlainText }

    }

  }

  Write-OutputFile -OutputParams $outputParams
}

function Invoke-Login{

  Write-Host "We will need to log into azure twice. Once for your home tenant where your resources will be deployed, and once to your newly created b2c tenant"
  Write-Host "Logging user into azure home tenant"
  az login

  Write-Host "User logged in successfully"

  $AzureSubscriptionId = $(az account show --query "id" -o tsv)
  $AzureSubscriptionName = $(az account show -s $AzureSubscriptionId --query "name" -o tsv)
  Write-Host "The default subscription from the current account is $AzureSubscriptionName -- $AzureSubscriptionId"
  $UseDefaultSubscriptionId = Read-Host -Prompt "Is this the subscription you'd like to use? (y/n) "

  if ($UseDefaultSubscriptionId -eq "n") {
    $AzureSubscriptionId = Read-Host -Prompt "Please provide the subscription ID of the subscription you'd like to use"
  }

  Write-Host "Using subscription ID ${AzureSubscriptionID}"
  az account set --subscription $AzureSubscriptionId

  Write-Host "Getting Access Token to Login to Powershell"

  $accountShowResponse = $(az account show --output json) | ConvertFrom-Json
  $accountId = $accountShowResponse.id
  $accessTokenResponse = $(az account get-access-token --output json) | ConvertFrom-Json
  $accessToken = $accessTokenResponse.accessToken

  Write-Host "Logging in to Az PowerShell"

  Connect-AzAccount -AccountId $accountId -AccessToken $accessToken -Subscription $AzureSubscriptionId

  Write-Host "Setting Az PowerShell Subscription to $AzureSubscriptionId"

  Get-AzSubscription -SubscriptionId $AzureSubscriptionId | Set-AzContext
  
  return @{
    SubscriptionId = $AzureSubscriptionId
    AccountId  = $accountId
    AccessToken = $accessToken
  }

}
function Get-UserInputParameters {
 
  $userInputParams = @{
    B2CTenantName = Read-Host "Please enter a name for the B2C tenant without the onmicrosoft.com suffix. (e.g. mytenant). Please note that tenant names must be globally unique."
    B2CTenantLocation = Read-Host "Please enter the location for the B2C Tenant to be created in. (United States', 'Europe', 'Asia Pacific', 'Australia)"
    CountryCode = Read-Host "Please enter the two letter country code for the B2C Tenant data to be stored in (e.g. 'US', 'CZ', 'DE'). See https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency for the list of available country codes."
    AzureResourceLocation = Read-Host "Please enter the location for the Azure Resources to be deployed (e.g. 'eastus', 'westus2', 'centraleurope'). Please run az account list-locations to see the available locations for your account."
    IdentityFrameworkResourceGroupName = Read-Host "Please enter the name of the Azure Resource Group to put the Identity Framework resources into. Will be created if it does not exist."
    SaasEnvironment = Read-Host "Please enter an environment name. Accepted values are: 'prd', 'stg', 'dev', 'tst'"
    ProviderName = Read-Host "Please enter a provider name. This name will be used to name the Azure Resources. (e.g. contoso, myapp). Max Length is 8 characters."
    InstanceNumber = Read-Host "Please enter an instance number. This number will be appended to most Azure Resources created. (e.g. 001, 002, 003)"
    SqlAdministratorLogin = Read-Host "Please enter the desired username for the SQL administrator account (e.g. sqladmin). Note: 'admin' is not allowed and will fail during the deployment step."
    SqlAdministratorLoginPassword = Read-Host -AsSecureString -Prompt "Please enter the desired password for the SQL administrator account."
    SelfSignedCertificatePassword = Read-Host -AsSecureString -Prompt "Please enter the desired password for the self-signed certificate that will be generated."
  }

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
    [string] $AzureSubscriptionId,
  
    # Under which Azure resource group will this B2C tenant reside.
    [string] $AzureResourceGroup

  )
  
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
    Write-Host "Resource Group $AzureResourceGroup does not exist. Creating..."
    az group create --name $AzureResourceGroup --location $AzureResourceLocation | Out-Null

    do {
      Write-Host "Waiting for 15 seconds for Resource Group creation..."
      Start-Sleep -Seconds 15
  
      az group show --name $AzureResourceGroup | Out-Null
    }
    while ($LastExitCode -ne 0)
  
    Write-Host "Resource Group Created Successfuly"
  }

  else {
    Write-Warning "Resource Group $AzureResourceGroup already exists. Skipping creation."
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
  az rest --method PUT --uri "https://management.azure.com$($resourceId)?api-version=2019-01-01-preview" --body $reqBody | Out-Null

  if ($LastExitCode -ne 0) {
    throw "Error on creating new B2C tenant!"
  }

  Write-Host "*** B2C Tenant creation started. It can take a moment to complete."

  do {
    Write-Host "Waiting for 30 seconds for B2C tenant creation..."
    Start-Sleep -Seconds 30

    az resource show --id $resourceId | Out-Null
  }
  while ($LastExitCode -ne 0)
  $tenantGuid = $(az resource show --id $resourceId --query "properties.tenantId" -o tsv)
  return $tenantGuid
}



function New-TrustFrameworkSigningKey {
  Write-Host "Creating new signing key..."
  $trustFrameworkKeySetName = "TokenSigningKeyContainer"
  try {
    $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
    New-MgTrustFrameworkKeySetKey -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -Kty "RSA" -Use "Sig"
  } catch {
    Write-Warning "Error on creating new signing key. Error: $_"
  }
  return $trustFrameworkKeySetName
}

function New-TrustFrameworkEncryptionKey {
  Write-Host "Creating new encryption key..."
  $trustFrameworkKeySetName = "TokenEncryptionKeyContainer"
  try {
    $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
    New-MgTrustFrameworkKeySetKey -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -Kty "RSA" -Use "Enc"
  } catch {
    Write-Warning "Error on creating new encryption key. Error: $_"
  }
    
  return $trustFrameworkKeySet
}

function New-TrustFrameworkClientCertificateKey {
  param (
    [string] $CertificateString,
    [Security.SecureString] $Pswd
  )
  Write-Host "Creating client certificate policy..."
  $trustFrameworkKeySetName = "RestApiClientCertificate"
  try {
    $trustFrameworkKeySet = New-MgTrustFrameworkKeySet -Id $trustFrameworkKeySetName
    $params = @{
      Key      = $CertificateString
      Password = ConvertFrom-SecureString -SecureString $Pswd -AsPlainText
    }
    Invoke-MgUploadTrustFrameworkKeySetPkcs12 -TrustFrameworkKeySetId $trustFrameworkKeySet.Id -BodyParameter $params

  } catch {
    Write-Warning "Error on creating client certificate policy. Error: $_"
  }

  return $trustFrameworkKeySet
}

function Import-IefPolicies {
  param (
    [string] $IEFPoliciesSourceDirectory = "Saas.IdentityProvider/policies",
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
    
    New-TrustFrameworkPolicy -PolicyId $basePolicy.Id -PolicyBody $basePolicy[0].Xml.OuterXml
   
     Import-ChildTrustFrameworkPolicies -CustomPolicyList $customPolicyList  -PolicyId $($basePolicy.Id)

     Write-Host "Policy Import Complete"

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
    New-TrustFrameworkPolicy -PolicyId $_.Id -PolicyBody $_.Xml.OuterXml
    Import-ChildTrustFrameworkPolicies -customPolicyList $customPolicyList -PolicyId $_.Id
  }

}

function New-TrustFrameworkPolicy{
  param (
    [string] $PolicyId,
    [string] $PolicyBody
  )
  Write-Host "Uploading policy $($PolicyId)..."

  $policy = Get-MgTrustFrameworkPolicy -Filter "Id eq '$($PolicyId)'" -Top 1
  if($null -eq $policy)
  {

    Invoke-MgGraphRequest -Uri "https://graph.microsoft.com/beta/trustFramework/policies" -Body $PolicyBody -ContentType "application/xml" -Method "POST"
  }
  else 
  {
    Invoke-MgGraphRequest -Uri "https://graph.microsoft.com/beta/trustFramework/policies/$($PolicyId)" -Body $PolicyBody -ContentType "application/xml" -Method "PUT"
  }
 
}
#Executes the Bicep template to install
function Invoke-IdentityBicepDeployment {
  param (
    [string] $IdentityFrameworkResourceGroupName,
    [string] $BicepTemplatePath = "Saas.Identity.IaC/main.bicep",
    [string] $B2CDomain,
    [string] $B2CInstanceName,
    [string] $B2cTenantId,
    [string] $PermissionsApiAppRegClientId,
    [string] $PermissionsApiAppRegClientSecret,
    [string] $PermissionsApiSelfSignedCertThumbprint,
    [string] $SaasProviderName,
    [string] $SaasEnvironment,
    [string] $SaasInstanceNumber,
    [string] $SqlAdministratorLogin,
    [securestring] $SqlAdministratorPassword
    
  )

  # # If running inside the docker container, fix the path
  # if ($null -eq $env:DOCKER -and $env:DOCKER -eq "true") {
  #   $BicepTemplatePath = "Saas.Identity.IaC/main.bicep"
  # }

  $params = @{
    azureAdB2cDomainSecretValue                     = $B2CDomain
    azureAdB2cInstanceSecretValue                   = $B2CInstanceName
    azureAdB2cTenantIdSecretValue                   = $B2cTenantId
    azureAdB2cPermissionsApiClientIdSecretValue     = $PermissionsApiAppRegClientId
    azureAdB2cPermissionsApiClientSecretSecretValue = $PermissionsApiAppRegClientSecret
    permissionsApiSslThumbprintSecretValue          = $PermissionsApiSelfSignedCertThumbprint
    saasProviderName                                = $SaasProviderName
    saasEnvironment                                 = $SaasEnvironment
    saasInstanceNumber                              = $SaasInstanceNumber
    sqlAdministratorLogin                           = $SqlAdministratorLogin
    sqlAdministratorLoginPassword                   = ConvertFrom-SecureString -SecureString $SqlAdministratorPassword -AsPlainText
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


function New-AsdkSelfSignedCertificate {
  param (
    [securestring] $CertificatePassword
  )

  try {
    Write-Host "Creating self signed certificate..."

    # We are using OpenSSL to create a self signed certificate because the PKI powershell module is not supported on Powershell Core yet

    ## Generate Certificate in .crt/.key format
    openssl req -newkey rsa:4096 -x509 -sha256 -days 365 -nodes -out certificate.crt -keyout certificate.key -subj "/CN=*.azurewebsites.net"

    # Convert Certificate to .pfx format with the private key
    # As mentioned in our documentation, self signed certificates are not suitable for anything other than testing. Do not use this certificate in production.
    $pswd = ConvertFrom-SecureString -SecureString $CertificatePassword -AsPlainText
    openssl pkcs12 -export -out selfSignedCertificate.pfx -inkey certificate.key -in certificate.crt -password pass:$pswd

    # Get the thumbprint of the generated certificate
    $pfxThumbprint = $(openssl pkcs12 -in selfSignedCertificate.pfx -nodes -passin pass:$pswd | openssl x509 -noout -fingerprint) -replace "SHA1 Fingerprint=", "" -replace ":", ""
    $pfxBytes = Get-Content "selfSignedCertificate.pfx" -AsByteStream
    $pfxString = [System.Convert]::ToBase64String($pfxBytes)
  }
  catch {
    Write-Error "An error occurred generating the self signed certificate: $_.Exception.Message"
    throw
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

  # Check to see if this app registration already exists by the name
  $createdApp = Get-MgApplication -ConsistencyLevel eventual -Filter "DisplayName eq '$($AppRegistrationData.DisplayName)'"  -Top 1
   
  # If it does exist, do not recreate it, but re-create the secret.
  if ($createdApp -ne $null) {
    $createdAppSecret = $null
    Write-Warning "App Registration '$($AppRegistrationData.DisplayName)' already exists, so we will not attempt to recreate it."

    if ($CreateSecret ) {
      Write-Warning "However, we still need to re-create its secret as it cannot be fetched after it has been created, and it is required for use later in the setup."
      Write-Host "Creating secret for app registration '$($AppRegistrationData.DisplayName)'"
      $createdAppSecret = Add-MgApplicationPassword -ApplicationId $createdApp.Id 
      
    }

    $createdSp = Get-MgServicePrincipal -ConsistencyLevel eventual -Filter "AppId eq '$($createdApp.AppId)'" -Top 1

    if ($createdSp -ne $null) {
      Write-Warning "Service Principal for app registration '$($AppRegistrationData.DisplayName)' already exists, so we will not attempt to recreate it."
    }
    else {
      Write-Host "Creating service principal for app registration '$($AppRegistrationData.DisplayName)'"
      $createdSp = New-MgServicePrincipal -AppId $createdApp.AppId -DisplayName $($createdApp.DisplayName)
      # Sleep to give time for graph consistency to update before moving on
      Start-Sleep -Seconds 3
    }

    return @{
      ClientSecret               = $createdAppSecret.SecretText
      AppRegistrationProperties  = $createdApp
      ServicePrincipalProperties = $createdSp
      AppRegistrationConfig      = $AppRegistrationData
    }

  }
  else {
    # If it does not exist, continue with the creation of the app registration.

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
    # Sleep to give time for graph consistency to update before moving on
    Start-Sleep -Seconds 3

    $newAppSecret = $null
    if ($CreateSecret) {
      $newAppSecretObject = Add-MgApplicationPassword -ApplicationId $newApp.Id 
      $newAppSecret = $newAppSecretObject.SecretText
    }

    # Also need to create the service principal for the app
    Write-Host "Creating Service Principal for App Registration $($AppRegistrationData.DisplayName)"
    $sp = New-MgServicePrincipal -AppId $newApp.AppId -DisplayName $newApp.DisplayName
    Write-Host "Created Service Principal for App Registration $($AppRegistrationData.DisplayName)"

    # Sleep to give time for graph consistency to update before moving on
    Start-Sleep -Seconds 3
    Write-Host "App Registration $($newApp.DisplayName) Created"
    return @{
      ClientSecret               = $newAppSecret
      AppRegistrationProperties  = $newApp
      ServicePrincipalProperties = $sp
      AppRegistrationConfig      = $AppRegistrationData
    }
  }
}

function New-SPAppRoleAssignment{
  param(
    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the application that consent is being granted on.")]
    [string] $ServicePrincipalId,
  
    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the API application for which consent is being granted for.")]
    [string] $ResourceId,

    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the API application for which consent is being granted for.")]
    [string] $AppRoleId
  )
  
  $appRoleAssignment = @{
    "principalId"= $ServicePrincipalId
    "resourceId"= $ResourceId
    "appRoleId"= $AppRoleId
    }
    try {
      New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $ServicePrincipalId -BodyParameter $appRoleAssignment | Format-List
    } catch {
      Write-Warning "There was an error when attempting to grant consent on the service principal with the ID of $ServicePrincipalId. It is most likely because this script has been run twice and the permission already exists. 
      If not, you will need to manually grant consent in the B2C Admin portal. Error: $_"
    }
  
}
function New-UserAppRoleAssignment{
  param(
    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the application that consent is being granted on.")]
    [string] $UserId,
  
    [Parameter(Mandatory = $true, HelpMessage = "The identifier of the API application for which consent is being granted for.")]
    [string] $ResourceId,
    [Parameter(Mandatory = $true, HelpMessage = "The app role id to be assigned to the user")]
    [string] $AppRoleId
  )
  
  $appRoleAssignment = @{
    "principalId"= $UserId
    "resourceId"= $ResourceId
    "appRoleId"= $AppRoleId
    }
  try {

    New-MgUserAppRoleAssignment -UserId $UserId -BodyParameter $appRoleAssignment | Format-List
  }
 catch {
  Write-Warning "There was an error when attempting to grant consent on the User with the ID of $UserId. It is most likely because this script has been run twice and the permission already exists. 
  If not, you will need to manually grant consent in the B2C Admin portal. Error: $_"
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
    [array] $ApiScopes

    # [Parameter(Mandatory = $false, HelpMessage = "The app roles to assign to the application.")]
    # [array] $AppRoles = $null
  )

  Write-Host "Granting consent for $ApiScopes on $ClientObjectId for $ApiObjectId"

  $currentDateTime = Get-Date
  $StartTime = $currentDateTime 
  $ExpiryTime = $currentDateTime.AddYears(5)


  $payload = @{
    ConsentType = "AllPrincipals"
    ClientId    = $ClientObjectId 
    ResourceId  = $ApiObjectId
    Scope       = $ApiScopes -Join " " #"tenant.delete tenant.write tenant.global.delete tenant.global.write tenant.read tenant.global.read"
    StartTime = $StartTime
    ExpiryTime = $ExpiryTime
    
  }

  $permissionGrant = Get-MgOauth2PermissionGrant -Filter "clientId eq '$($ClientObjectId)' and resourceId eq '$($ApiObjectId)'and ConsentType eq 'AllPrincipals'" -Top 1

  if ($null -ne $permissionGrant) {
    Write-Warning "Consent already exists for $($ApiScopes -Join " ") on $($ClientObjectId) for $($ApiObjectId). Skipping Creation."
  }
  else {

    try {
      New-MgOauth2PermissionGrant -BodyParameter $payload 
      Write-Host "Consent granted"
    }
    catch {
      Write-Error "Error granting admin consent: $_"
      throw
    }
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
    [string] $SaasAppFQDN,
    [Parameter(Mandatory = $true, HelpMessage = "The current user object id")]
    [string] $CurrentB2CUserId
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
  $msGraphScopes = @('offline_access', 'openid')

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

  # Grant admin consent on the signupadmin app for the admin scopes
  New-AdminConsent -ClientObjectId $adminAppReg.ServicePrincipalProperties.Id -ApiObjectId $msGraphServicePrincipal.Id -ApiScopes $msGraphScopes
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $adminAppReg.ServicePrincipalProperties.Id -AppRoleId "00000000-0000-0000-0000-000000000000"

  ############## Signup Admin App Registration ##############

  $GlobalAdminAppRoleId = New-Guid
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
    PublicClient           = @{ }
    Web                    = @{ 
      ImplicitGrantSettings = @{
        EnableAccessTokenIssuance = $true
        EnableIdTokenIssuance     = $true
      }
      LogoutUrl             = "$($SignupAdminFQDN)/signout-oidc"
      RedirectUris          = @("$($SignupAdminFQDN)/signin-oidc")
    }
    AppRoles               = @(
      @{
        AllowedMemberTypes = @("User")
        Description        = "Global Admin - Web"
        DisplayName        = "Global Admin - Web"
        Id                 = $GlobalAdminAppRoleId
        Value              = "GlobalAdmin"
      }
    )

  }

  # Create the App Registration
  $signupAdminAppReg = New-AppRegistration -AppRegistrationData $signupAdminAppRegConfig -CreateSecret $true
  # Get the scopes from the admin app registration
  $adminScopes = $adminAppRegConfig.OAuth2PermissionScopes | ForEach-Object { $_.Value }
  
  # Grant admin consent on the signupadmin app for the admin scopes
  New-AdminConsent -ClientObjectId $signupAdminAppReg.ServicePrincipalProperties.Id -ApiObjectId $adminAppReg.ServicePrincipalProperties.Id -ApiScopes $adminScopes
  New-AdminConsent -ClientObjectId $signupAdminAppReg.ServicePrincipalProperties.Id -ApiObjectId $msGraphServicePrincipal.Id -ApiScopes $msGraphScopes
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $signupAdminAppReg.ServicePrincipalProperties.Id -AppRoleId $GlobalAdminAppRoleId

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

  # Grant Admin consent for the scopes and app roles to MS Graph API
  New-AdminConsent -ClientObjectId $permissionsAppReg.ServicePrincipalProperties.Id `
    -ApiObjectId $msGraphServicePrincipal.Id `
    -ApiScopes @("offline_access", "openid") `
    # -AppRoles $permissionsAppRegGraphAppRoles `
  New-SPAppRoleAssignment -ServicePrincipalId $permissionsAppReg.ServicePrincipalProperties.Id -ResourceId $msGraphServicePrincipal.Id -AppRoleId "df021288-bdef-4463-88db-98f22de89214"
  New-SPAppRoleAssignment -ServicePrincipalId $permissionsAppReg.ServicePrincipalProperties.Id -ResourceId $msGraphServicePrincipal.Id -AppRoleId "9a5d68dd-52b0-4cc2-bd40-abcf44ac3a30"
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $permissionsAppReg.ServicePrincipalProperties.Id -AppRoleId "00000000-0000-0000-0000-000000000000"


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
  New-AdminConsent -ClientObjectId $saasAppAppReg.ServicePrincipalProperties.Id -ApiObjectId $msGraphServicePrincipal.Id -ApiScopes $msGraphScopes
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $saasAppAppReg.ServicePrincipalProperties.Id -AppRoleId "00000000-0000-0000-0000-000000000000"
  
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
  New-AdminConsent -ClientObjectId $iefAppReg.ServicePrincipalProperties.Id -ApiObjectId $msGraphServicePrincipal.Id -ApiScopes $msGraphScopes
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $iefAppReg.ServicePrincipalProperties.Id -AppRoleId "00000000-0000-0000-0000-000000000000"
  
  
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
  New-AdminConsent -ClientObjectId $iefProxyAppReg.ServicePrincipalProperties.Id -ApiObjectId $msGraphServicePrincipal.Id -ApiScopes $msGraphScopes
  New-UserAppRoleAssignment -UserId  $CurrentB2CUserId -ResourceId $iefProxyAppReg.ServicePrincipalProperties.Id -AppRoleId "00000000-0000-0000-0000-000000000000"
 

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
  param (
    [hashtable] $OutputParams,
    [string] $OutputFile = "parameters.json",
    [string] $OutputDirectory = "/data"
  )

  $outputJson = $OutputParams | ConvertTo-Json
  Write-Host "Output parameters file to $OutputFile"


  if (Test-Path -Path $OutputDirectory) {
    Write-Host "A data directory has been mounted. Writing output file to $OutputDirectory/$OutputFile"
    $outputJson > "$OutputDirectory/$OutputFile"
  }
  else {
    Write-Host "No data directory was detected. If running this script via docker, you will need to copy this file out of the container onto your host machine."
    Write-Host "ie: docker cp <container_id>:/data/parameters.json /host/path/parameters.json"
    $outputJson > "./$OutputFile"
  }

}

New-SaaSIdentityProvider

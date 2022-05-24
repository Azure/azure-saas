# Windows PowerShell and PowerShell Core are supported.
# - Microsoft.Graph PowerShell module needs to be installed.
# - Azure CLI needs to be installed and authenticated for the owning tenant.
#
# Usage:
# - dot-source in a PS script: . ./Create-AzureB2C.ps1
# - invoke individual functions, or the main one: Initialize-B2CTenant -B2CTenantName mytenant -ResourceGroupName myrg -Location "Europe" -CountryCode "CZ"

function Deploy-SaaSIdentityProvider {
    [CmdletBinding()] # indicate that this is advanced function (with additional params automatically added)
    param (
      [Parameter(Mandatory = $true, HelpMessage = "B2C tenant name, without the '.onmicrosoft.com'.")]
      [string] $B2CTenantName,
      
      [Parameter(Mandatory = $true, HelpMessage = "Name of the Azure Resource Group to put the B2C resource into. Will be created if it does not exist.")]
      [string] $ResourceGroupName,
  
      [string] $Location = "United States",
      
      [Parameter(HelpMessage = "Two letter country code (e.g. 'US', 'CZ', 'DE'). https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency")]
      [string] $CountryCode = "US",
    
      [Parameter(HelpMessage = "The name of the SKU for the tenant. (e.g. 'PremiumP1', 'PremiumP2', 'Standard'). https://azure.microsoft.com/en-us/pricing/details/active-directory/external-identities/")]
      [string] $SKU = "PremiumP1"
    )
  
    if (Get-Module -ListAvailable -Name Microsoft.Graph) {
      Write-Host "Module Microsoft.Graph exists."
    }
    else {
        throw "Module Microsoft.Graph is not installed yet. Please install it first! Run 'Install-Module Microsoft.Graph'."
    }

    #Create App Service and Key Vault
    Write-Host "Creating App Service and Key Vault..."

  
    # Create the B2C tenant resource in Azure
    New-AzureADB2CTenant `
      -B2CTenantName $B2CTenantName `
      -Location $Location `
      -CountryCode $CountryCode `
      -AzureResourceGroup $ResourceGroupName
      -Sku $SKU
  
    # Call the init API
    Invoke-TenantInit `
     -B2CTenantName $B2CTenantName
  
    Write-Host "Interactive login to the Graph API. Watch for a newly opened browser window (or device flow instructions) and complete the sign in."
    # Interactive login, so that we don't have to create a separate service principal and handle secrets.
    # Make sure that the user has administrative permissions in the tenant.
    Connect-MgGraph -TenantId "$($B2CTenantName).onmicrosoft.com" -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "Directory.AccessAsUser.All", "Directory.ReadWrite.All"
  
    #Setup IEF
    Write-Host "Setting up Identity Experience Framework..."
    New-IefPolicies -B2CTenantName $B2CTenantName
    
    
    

  
  }

  function New-AzureADB2CTenant {
    param(
      # Tenant name without the '.onmicrosoft.com' part.
      [string] $B2CTenantName,
  
      # Can be one of 'United States', 'Europe', 'Asia Pacific', or 'Australia' (preview).
      [Parameter()]
      [ValidateSet('United States','Europe','Asia Pacific', 'Australia')]
      [string] $Location,
  
      # Where data resides. Two letter country code (e.g. 'US', 'CZ', 'DE').
      # Valid country codes are listed here: https://docs.microsoft.com/en-us/azure/active-directory-b2c/data-residency
      [string] $CountryCode,
  
      # Under which Azure subscription will this B2C tenant reside. If not provided, use the current subscription from Azure CLI.
      [string] $AzureSubscriptionId = $null,
  
      # Under which Azure resource group will this B2C tenant reside.
      [string] $AzureResourceGroup,

      [string] $Sku

    )
  
    if (!$AzureSubscriptionId) {
      Write-Host "Getting subscription ID from the current account..."
      $AzureSubscriptionId = $(az account show --query "id" -o tsv)
      Write-Host $AzureSubscriptionId
    }
  
    $aadProviderRegState = $(az provider show -n Microsoft.AzureActiveDirectory --query "registrationState" -o tsv)
    if($aadProviderRegState -ne "Registered")
    {
      Write-Host "Resource Provider 'Microsoft.AzureActiveDirectory' not registered yet. Registering now..."
      az provider register --namespace Microsoft.AzureActiveDirectory
  
      while($(az provider show -n Microsoft.AzureActiveDirectory --query "registrationState" -o tsv) -ne "Registered")
      {
        Write-Host "Resource Provider registration not yet finished. Waiting..."
        Start-Sleep -Seconds 10
      }
      Write-Host "Resource Provider registration finished."
    }
  
    Write-Host "Checking if Resource Group $AzureResourceGroup exists..."
    $checkRg = az group exists --name $AzureResourceGroup | ConvertFrom-Json
  
    if($LastExitCode -ne 0)
    {
        throw "Error on using Azure CLI. Make sure the CLI is installed, up-to-date and you are signed in. Run 'az login' to sign in."
    }
  
    if (!$checkRg) {
      Write-Warning "Resource Group $AzureResourceGroup does not exist. Creating..."
      az group create --name $AzureResourceGroup --location "northeurope" # Everybody likes Ireland, so we put the RG there if it does not exist
    }
  
    $resourceId = "/subscriptions/$AzureSubscriptionId/resourceGroups/$AzureResourceGroup/providers/Microsoft.AzureActiveDirectory/b2cDirectories/$B2CTenantName.onmicrosoft.com"
  
    # Check if tenant already exists
    Write-Host "Checking if tenant '$B2CTenantName' already exists..."
    az resource show --id $resourceId | Out-Null
    if($LastExitCode -eq 0) # No error means, the resource exists
    {
      Write-Warning "Tenant '$B2CTenantName' already exists. Not attempting to recreate it."
      return
    }
  
    $reqBody=
    @"
    {
      "location":"$($Location)",
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

  if($LastExitCode -ne 0)
  {
      throw "Error on creating new B2C tenant!"
  }

  Write-Host "*** B2C Tenant creation started. It can take a moment to complete."

  do
  {
    Write-Host "Waiting for 30 seconds for B2C tenant creation..."
    Start-Sleep -Seconds 30

    az resource show --id $resourceId
  }
  while($LastExitCode -ne 0)
  
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
    $managementAccessToken = $(az account get-access-token --tenant "$B2CTenantId" --query accessToken -o tsv)
  
    # Invoke tenant initialization which happens through the portal automatically.
    # Ref: https://stackoverflow.com/questions/67706798/creation-of-the-b2c-extensions-app-by-script
    Write-Host "Invoking tenant initialization..."
    Invoke-WebRequest -Uri "https://main.b2cadmin.ext.azure.com/api/tenants/GetAndInitializeTenantPolicy?tenantId=$($B2CTenantId)&skipInitialization=false" `
      -Method "GET" `
      -Headers @{
        "Authorization" = "Bearer $($managementAccessToken)"
      }
  }

  function New-IefPolicies{
    param (
        [string] $B2CTenantName
      )
        Install-ModuleIfNotInstalled -moduleName "IefPolicies" -minimalVersion "3.1.4"
        Connect-IefPolicies "$($B2CTenantName)"
        Initialize-IefPolicies 
        Write-Host "IefPolicies initialized"

        #Create Key with certificate

  }

  function Import-IefPolicies{
   

  }

  #Exectues the Bicep template to install
  function Install-SaaSIdentityProvider
  {


  }

  function Install-AppRegistration
  {
    
  }
  function Install-ModuleIfNotInstalled{
      param(
    [string] [Parameter(Mandatory = $true)] $moduleName,
    [string] $minimalVersion
    ) 
    $module = Get-Module -Name $moduleName -ListAvailable |`
        Where-Object { $null -eq $minimalVersion -or $minimalVersion -lt $_.Version } |`
        Select-Object -Last 1
    if ($null -ne $module) {
         Write-Verbose ('Module {0} (v{1}) is available.' -f $moduleName, $module.Version)
    }
    else {
        Import-Module -Name 'PowershellGet'
        $installedModule = Get-InstalledModule -Name $moduleName -ErrorAction SilentlyContinue
        if ($null -ne $installedModule) {
            Write-Verbose ('Module [{0}] (v {1}) is installed.' -f $moduleName, $installedModule.Version)
        }
        if ($null -eq $installedModule -or ($null -ne $minimalVersion -and $installedModule.Version -lt $minimalVersion)) {
            Write-Verbose ('Module {0} min.vers {1}: not installed; check if nuget v2.8.5.201 or later is installed.' -f $moduleName, $minimalVersion)
            #First check if package provider NuGet is installed. Incase an older version is installed the required version is installed explicitly
            if ((Get-PackageProvider -Name NuGet -Force).Version -lt '2.8.5.201') {
                Write-Warning ('Module {0} min.vers {1}: Install nuget!' -f $moduleName, $minimalVersion)
                Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Scope CurrentUser -Force
            }        
            $optionalArgs = New-Object -TypeName Hashtable
            if ($null -ne $minimalVersion) {
                $optionalArgs['RequiredVersion'] = $minimalVersion
            }  
            Write-Warning ('Install module {0} (version [{1}]) within scope of the current user.' -f $moduleName, $minimalVersion)
            Install-Module -Name $moduleName @optionalArgs -Scope CurrentUser -Force -Verbose
        } 
    }
}
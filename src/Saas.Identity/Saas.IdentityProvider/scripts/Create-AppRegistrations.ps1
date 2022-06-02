function New-AppRegistration {
    param (
        [Parameter(Mandatory = $true, HelpMessage = "A Hash table of data to be added to be sent with the app registration creation request.")]
        [hashtable] $AppRegistrationData,

        [Parameter(Mandatory = $false, HelpMessage = "Indicates whether to create a secret for the app registration")]
        [bool] $CreateSecret = $false
    )
    $newApp = New-MgApplication `
    -DisplayName $AppRegistrationData.DisplayName `
    -Api @{
        Oauth2PermissionScopes = $AppRegistrationData.OAuth2PermissionScopes
    } `
    -IdentifierUris $AppRegistrationData.IdentifierUris `
    -RequiredResourceAccess $AppRegistrationData.RequiredResourceAccess `

    $newAppSecret = $null
    if ($CreateSecret) {
        $newAppSecret = Add-MgApplicationPassword -ApplicationId $newApp.Id | Select-Object SecretText
    }

    return @{
        Id = $newApp.Id
        Name = $newApp.DisplayName
        AppId = $newApp.AppId
        ClientSecret = $newAppSecret
        Properties = $newApp
    }
}

function Initialize-AppRegistrations {
    param(
        [Parameter(Mandatory = $true, HelpMessage = "The Tenant Identifier")]
        [string] $TenantId
    )
    $adminAppRegConfig = @{
        DisplayName = "asdk-admin-api"
        IdentifierUris = @("https://$($TenantId)/$(New-Guid)")
        OAuth2PermissionScopes = @(
            @{
                AdminConsentDisplayName  = "Allows deletion of tenants";
                AdminConsentDescription   = "Allows deletion of tenants";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.global.delete";
            },
            @{
                AdminConsentDisplayName  = "Allow deletion of user's tenants";
                AdminConsentDescription   = "Allow deletion of user's tenants";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.delete";
            },
            @{
                AdminConsentDisplayName  = "Write to user's tenants";
                AdminConsentDescription   = "Write to user's tenants";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.write";
            },
            @{
                AdminConsentDisplayName  = "Write to all tenants";
                AdminConsentDescription   = "Write to all tenants";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.global.write";
            },
            @{
                AdminConsentDisplayName  = "Allow reading tenant data for current user";
                AdminConsentDescription   = "Allow reading tenant data for current user";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.read";
            },
            @{
                AdminConsentDisplayName  = "Allow reading of tenant data across all tenants";
                AdminConsentDescription   = "Allow reading of tenant data across all tenants";
                Id = New-Guid;
                Type = "Admin";
                Value = "tenant.global.read";
            }
        )
        RequiredResourceAccess = @()
    }

    $adminAppReg = New-AppRegistration -AppRegistrationData $adminAppRegConfig

    $signupAdminAppRegConfig = @{
        DisplayName = "asdk-signupadmin-app"
        IdentifierUri = @("https://$($TenantId)/$(New-Guid)")
        OAuth2PermissionScopes = @()
        RequiredResourceAccess = @{
            ResourceAppId = $adminAppReg.AppId
            ResourceAccess = $adminAppRegConfig.OAuth2PermissionScopes | ForEach-Object { @{Id = $_.Id;Type = "Scope"} }
        }

    }

    $signupAdminAppReg = New-AppRegistration -AppRegistrationData $signupAdminAppRegConfig -CreateSecret $true

    return @{
        AdminAppReg = $adminAppReg
        SignupAdminAppReg = $signupAdminAppReg
    }
}

$B2CTenantName = "lptestb2ctenant01"
Connect-MgGraph -TenantId "$($B2CTenantName).onmicrosoft.com" -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "Directory.AccessAsUser.All", "Directory.ReadWrite.All", "TrustFrameworkKeySet.ReadWrite.All"

$ret = Initialize-AppRegistrations -TenantId "$($B2CTenantName).onmicrosoft.com"

Write-Host "Done"
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
        -Web $AppRegistrationData.Web `
        -AppRoles $AppRegistrationData.AppRoles `

    $newAppSecret = $null
    if ($CreateSecret) {
        $newAppSecret = Add-MgApplicationPassword -ApplicationId $newApp.Id | Select-Object SecretText
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
                ResourceId = $role.ResourceId #"8e881353-1735-45af-af21-ee1344582a4d"
                AppRoleId = $role.AppRoleId 
            }
            New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $role.PrincipalId -BodyParameter $params
            Write-Host "Assigned $($role.AppRoleId) to $($role.PrincipalId)"
        }
    }
    
    $payload = @{
        ConsentType = "AllPrincipals"
        ClientId    = $ClientObjectId #"0b7224ff-15c8-4fbf-81b2-15ab3780f452" # object id of signup admin
        ResourceId  = $ApiObjectId #"e0a6143a-1135-400f-87ac-6e2f76b967e6" # object id of asdk-adminapi
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

function Initialize-AppRegistrations {
    param(
        [Parameter(Mandatory = $true, HelpMessage = "The Tenant Identifier")]
        [string] $TenantId,
        [Parameter(Mandatory = $true, HelpMessage = "The estimated FQDN for the signupadmin azure app service")]
        [string] $SignupAdminFQDN,
        [Parameter(Mandatory = $true, HelpMessage = "The estimated FQDN for the saas app azure app service")]
        [string] $SaaSAppFQDN
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

    ############## Admin API App Registration ##############

    $adminAppRegConfig = @{
        DisplayName            = "asdk-admin-api"
        IdentifierUris         = @("https://$($TenantId)/$(New-Guid)")
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
        IdentifierUri          = @("https://$($TenantId)/$(New-Guid)")
        OAuth2PermissionScopes = @()
        RequiredResourceAccess = @(@{
                ResourceAppId  = $adminAppReg.AppRegistrationProperties.AppId
                ResourceAccess = $adminAppRegConfig.OAuth2PermissionScopes | ForEach-Object { @{Id = $_.Id; Type = "Scope" } }
            },
            $msGraphAccess # Add Default Microsoft Graph permissions
        )
        PublicClient           = @{ redirectUris = @("$($SignupAdminFQDN)/signin-oidc") }
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
        IdentifierUri          = @("https://$($TenantId)/$(New-Guid)")
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
            ResourceId = "01c2693d-f03b-404e-869f-14f5a396c0a9" # MS Graph Resource ID
            AppRoleId =  "df021288-bdef-4463-88db-98f22de89214" # App Role ID for Application.Read.All
        },
        @{
            PrincipalId = $permissionsAppReg.ServicePrincipalProperties.Id
            ResourceId = "01c2693d-f03b-404e-869f-14f5a396c0a9" # MS Graph Resource ID
            AppRoleId =  "9a5d68dd-52b0-4cc2-bd40-abcf44ac3a30" # App Role ID for User.Read.All
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
        IdentifierUri          = @("https://$($TenantId)/$(New-Guid)")
        OAuth2PermissionScopes = @()
        RequiredResourceAccess = @(@{
                ResourceAppId  = $adminAppReg.AppRegistrationProperties.AppId
                ResourceAccess = @($adminAppRegConfig.OAuth2PermissionScopes | Where-Object {$_.Value -eq "tenant.read"} | ForEach-Object { @{Id = $_.Id; Type = "Scope" } })
            },
            $msGraphAccess # Add Default Microsoft Graph permissions
        )
        PublicClient           = @{ redirectUris = @("$($SaaSAppFQDN)/signin-oidc") }
        Web                    = @{ 
            ImplicitGrantSettings = @{
                EnableAccessTokenIssuance = $true
                EnableIdTokenIssuance     = $true
            }
            LogoutUrl             = "$($SaaSAppFQDN)/signout-oidc"
            RedirectUris          = @("$($SaaSAppFQDN)/signin-oidc")
        }
        AppRoles               = @{}

    }

    # Create the App Registration
    $saasAppAppReg = New-AppRegistration -AppRegistrationData $saasAppAppRegConfig -CreateSecret $true
    # Get the scopes from the admin app registration
    $adminScopesForSaasApp = @($adminAppRegConfig.OAuth2PermissionScopes | Where-Object {$_.Value -eq "tenant.read"} | ForEach-Object { $_.Value })
    # Grant admin consent on the signupadmin app for the admin scopes
    New-AdminConsent -ClientObjectId $saasAppAppReg.ServicePrincipalProperties.Id -ApiObjectId $adminAppReg.ServicePrincipalProperties.Id -ApiScopes $adminScopesForSaasApp



    Write-Host "App Registrations Created"
    
    return @{
        AdminAppReg       = $adminAppReg
        SignupAdminAppReg = $signupAdminAppReg
        PermissionsAppReg = $permissionsAppReg
        SaasAppAppReg     = $saasAppAppReg
        #IEFAppREg 
    }
}


$B2CTenantName = "lptestb2ctenant01"
Connect-MgGraph -TenantId "$($B2CTenantName).onmicrosoft.com" -Scopes "User.ReadWrite.All", "Application.ReadWrite.All", "Directory.AccessAsUser.All", "Directory.ReadWrite.All", "TrustFrameworkKeySet.ReadWrite.All"

$ret = Initialize-AppRegistrations `
-TenantId "$($B2CTenantName).onmicrosoft.com" `
-SignupAdminFQDN "https://landonlptest.azurewebsites.net" `
-SaasAppFQDN "https://saasapplptest.azurewebsites.net" `

# Write-Host "Done"



Write-Host "done"

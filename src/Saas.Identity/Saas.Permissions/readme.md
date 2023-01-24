# SaaS Permissions Service

## Overview

Section contains the ASDK Permissions Services API for handling role-based authorization of user. The service [depends](https://azure.github.io/azure-saas/components/identity/permissions-service#dependencies) on the Identity Foundation that was deployed a spart of the Identity Foundation and on the [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/use-the-api).

For a complete overview, please see the [SaaS Permissions Service](https://azure.github.io/azure-saas/components/identity/permissions-service/) page in the documentation site.

## How to Run Locally

Once configured, this service presents a web api service exposing endpoints to perform CRUD operations on user permission settings.

The SaaS Permissions Service API can be run locally during development, testing and learning.

### Requirements

To run the web api, you must have the following installed on your developer machine:

- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/download).
- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [ASP.NET Core 7.0](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)

You also need a deployed instance of the [Identity Framework](https://azure.github.io/azure-saas/quick-start/). For details visit the [Deploying the Identify Foundation Services readme](../Saas.Identity.Provider/readme.md).

###  App Configuration and Settings

To manage settings securely and efficiently, settings are being stored in [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview), while secrets and certificates are being stored in [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview). Furthermore, secrets are represented with a reference (an URI) in Azure App Configuration, while the actual secret is kept safely and securely in Azure Key Vault. 

All necessary settings, certificates and secrets needed for running the SaaS Permissions API, where automatically created and provisioned during the deployment of the Identity Framework, during the ASDK Identity Foundation deployment. 

### Setting up the local development environment

For running the SaaS Permission API Service in a local development environment, we need a few extra steps to set up access to the provisioned Azure App Configuration and the Azure Key Vault service, to your local development environment. 

#### Access and Permissions to Azure Key Vault 

For access to Azure Key Vault, we will rely on [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) to provide the needed access token. For this to work, you should open a terminal from within Visual Studio (or Visual Studio Code) and run these commands:

```bash
az account show # use this to see if you're already logged into your Azure tanent, if not use the next command to login
az login
```

Code have been added to the [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0) project leveraging the local Azure CLI environment. You'll find this code in in `Program.cs`:

```csharp
if (builder.Environment.IsDevelopment())
{
    ...
    AzureCliCredential credential = new();

    builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(appConfigurationconnectionString)
                .ConfigureKeyVault(kv => kv.SetCredential(new ChainedTokenCredential(credential)))
            .Select(KeyFilter.Any, version));
    ...
}
```

#### Access and Permissions to Azure App Configuration

To manage access to Azure App Configuration, securely, we need one more thing. 

From your local development environment, you can leverage the Dotnet [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows), to securely store a `connection string` allowing the local app to access the provisioned Azure App Configuration instance. 

This is a two step process:

1. The Azure App Configuration `connection string` can either be found in the Azure Portal by navigating to your Azure App Configuration instance, that was deployed as part of the Identity Foundation. Or, it can be obtained using this az cli command: 

```bash
az appconfig credential list --name "<name of your azure app configuration> --query [0].connectionString"
```

In the Azure Portal you can find the connection string here:

![image-20230105174952120](assets/readme/image-20230105174952120.png)

2. To add the `connection string` to the Secret Manager, run these commands in a terminal in the root directory of the project:

```cmd
dotnet user-secrets init #initialized your Secret Manager for the project.
dotnet user-secrets set ConnectionStrings:AppConfig "<your_azure_app_config_connection_string>"
```

> Tip: For more details on connecting a local development environment to Azure App Configuration please see: [Connect to the App Configuration store](https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-aspnet-core-app?tabs=core6x#connect-to-the-app-configuration-store). 

### Accessing the Azure SQL Server data from your developer environment

The following is good to know, for ensuring that your development environment has access to the SQL Server Database, that was deployed as part of the Identity Foundation.

During he deployment, the deployment script takes note of the public IP address of the developer machine running the script. The script then adds this specific IP address to an *allowed firewall rule*. 

Adding your public IP address is essential for your local development environment to be able to run. By default the configuration of the SQL Server only allows network access from IP addresses of services running *inside* the Azure environment. This default network security setting is great for production, however since your local development environment is not likely running inside the Azure environment, this firewall restriction may get in the way.

You may also want to work on you project from multiple locations and development environments, in which case you will need to make changes to the firewall rules of Azure SQL Server, allowing these additional public IPs to access the database.

To do this, please do the following:

1. You can identity the global IP address by running this bash command on MacOS, Linux or on Windows 10/11 from within a [WSL](https://learn.microsoft.com/en-us/windows/wsl/install) terminal session:

```bash
dig +short myip.opendns.com @resolver1.opendns.com
```

2. Visit the Azure portal and add the global IP address of the computing you are running the Permission Service from. 

![image-20230107210713030](assets/readme/image-20230107210713030.png)

## Running the Permissions Service API

After all of the above have been set up, you're now ready to build and run the SaaS Permissions Services. When you do, a browser will open and load a Swagger Page:

![image-20230112000806828](assets/readme/image-20230112000806828.png)

Now *try it out* and run the `GetTenantUsers` API. It will take approx. 20-40 seconds to complete the request the first time you run it, because the app will need to authenticate itself, including getting a signed assertion from Key Vault. 

> Tip: After the first run the access token is cached, so if you run the request a second time it will be much faster. 

Enter the `tenantId` of your Azure B2C Tenant (i.e., the one that was created as part of deploying the Identity Foundation). You'll find it in the `config.json` file at `.deployment.azureb2c.tenantId`.

![image-20230112001210631](assets/readme/image-20230112001210631.png)

## How  to Deploy to Azure

For deploying to Azure we suggest leveraging [GitHub Actions](https://github.com/features/actions). 



## How to debug in Azure

We're going to deploy for Windows, rather than Linux, because the Windows remote debugging is the most seamless, and we're expecting that you'll want to remote debug into the Azure instance to explore and see how the app runs there. That said, nothing prevents you from deploying to Linux for production, since ASP.NET Core 7 runs equally well on Linux.

For more on remote debugging with Visual Studio 2022 see: [Remote Debug ASP.NET Core on Azure App Service - Visual Studio (Windows) | Microsoft Learn](https://learn.microsoft.com/en-us/visualstudio/debugger/remote-debugging-azure-app-service?view=vs-2022). 



## How to Debug in Azure

After deploying to Azure everything should work. But what if it doesn't? We could attach a debugger then. Sure, but what if the ASP.NET Core app isn't even starting? 

### My app won't start

Command line to the rescue. 

![image-20230120213644846](assets/readme/image-20230120213644846.png)

<TO DO>
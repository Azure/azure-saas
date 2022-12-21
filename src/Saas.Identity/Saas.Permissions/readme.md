# SaaS.Permissions.Service

## Module Overview

This project contains an API for the role-based authorization of user actions. It is fully self-contained such that it includes complete copies of all necessary classes for operation. However, keep in mind that some functionality within the API does have [dependencies](https://azure.github.io/azure-saas/components/identity/permissions-service#dependencies) on SQL Server Database that was deployed a spart of the Identity Foundation and on the [Microsoft Graph API](https://learn.microsoft.com/en-us/graph/use-the-api).

 For a complete overview, please see the [SaaS.Permissions.Service](https://azure.github.io/azure-saas/components/identity/permissions-service/) page in our documentation site.

## How to Run Locally

Once configured, this app presents an api service which exposes endpoints to perform CRUD operations on user permission data. It may be run locally during development of service logic and for regenerating its included NSwag api client. (An NSwag file is included in the Admin project to generate its client.)

### Requirements

To run the web api, you must have the following installed on your machine:

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [ASP.NET Core 7.0](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) (recommended) or [Visual Studio Code](https://code.visualstudio.com/download).

You also need a a deployed [Identity Framework](https://azure.github.io/azure-saas/quick-start/) instance.

### Development Tools

- [NSwag](https://github.com/RicoSuter/NSwag) - An NSwag configuration file has been included to generate an appropriate client from the included Admin project.
    *Consumed By:*
    - [Saas.Admin.Service](../../Saas.Admin)

###  App Configuration and Settings

This project rely on settings being stored in [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview) as well as secrets and certificates being stored in [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview). Secrets are represented with a reference (URI) in Azure App Configuration pointing to the actual secret in Azure Key Vault. 

Settings, certificates and secrets where automatically created and provisioned during the Azure deployment of the Identity Framework. For running the SaaS Permission Service in a local development environment, we still need a few extra steps to set up permissions to access both Azure App Configuration and Azure Key Vault from your computer. 

#### Access and Permissions to Azure Key Vault 

For local development access to Azure Key Vault, we will rely on Azure CLI to provide access tokens. For this to work, you should open a terminal in Visual Studio or Visual Studio Code and run these commands to log into your main Azure tenant (i.e., not your Azure B2C tenant):

```bash
az account show # use this to see if you're already logged into your Azure tanent, if not use the next command to login
az login
```

Not that you are logged in, we've configured your development environment to leverage your Azure CLI credentials by using the `AzureCliCredential` class in the `Azure.Identity` library. In the [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0) code this looks like this, in `Program.cs`:

```csharp
if (builder.Environment.IsDevelopment())
{
    ...
    builder.Configuration.AddAzureAppConfiguration(options =>
                options.Connect(connectionString)
                    .ConfigureKeyVault(kv => kv.SetCredential(new ChainedTokenCredential(
                        new AzureCliCredential())))...
}
```

#### Access and Permissions to Azure App Configuration

To manage access to Azure App Configuration securely we need one more thing. From our development environment, we will leverage the Dotnet [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows). To do this, is a two step process please first get the Azure App Configuration `connection string`, then this string to the Secret Manager:

1. The Azure App Configuration `connection string` can be found in the Azure Portal by navigating to your Azure App Configuration instance, that was deployed as part of the Identity Framework. Or, it can be obtained from the command line by running this az cli command from a terminal logged into your Azure tenant: 

```bash
az appconfig credential list --name "<name of your azure app configuration> --query [0].connectionString"
```

In the Azure Portal it looks like this:

![image-20230105174952120](assets/readme/image-20230105174952120.png)

For more details on connecting a local development environment to Azure App Configuration please see: [Connect to the App Configuration store](https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-aspnet-core-app?tabs=core6x#connect-to-the-app-configuration-store). 

2. To add the `connection string` to the Secret Manager, run these commands in a terminal in the root directory of the project:

```cmd
dotnet user-secrets init #initialized your Secret Manager for the project.
dotnet user-secrets set ConnectionStrings:AppConfig "<your_azure_app_config_connection_string>"
```

### Accessing the Azure SQL Server data from your developer environment

The following is good to know, to ensure that you development environment has access to the SQL Server Database that was deployed as part of the Identity Foundation.

During he deployment, the script first takes note of the public IP address of the developer machine running the script. Then the script adds this IP address to the firewall rules of the database running in Azure. This step is essential, since the default configuration of the SQL Server is to only allow network access from IP addresses running **inside** the Azure environment. This is great for production, but since you development environment is not running inside the Azure environment, this firewall restriction might get in the way.

Also, you may want to work on you project from multiple locations and dev-machines. In those cases you will need to make further changes to the firewall rules of Azure SQL Server. 

To do this, please do the following this single step:

1. Visit the Azure portal and add the global IP address of the computing you are running the Permission Service from. 

You can find the global IP address by running this bash command on MacOS or on Windows 10/11 from a GNU Linux terminal:

```bash
dig +short myip.opendns.com @resolver1.opendns.com
```

![image-20230107210713030](assets/readme/image-20230107210713030.png)

### Running the Permissions Service API

1. After all of the above have been set up, you're ready to compile and run from your dev PC, which will open a browser and load a Swagger Page. 

![image-20230112000806828](assets/readme/image-20230112000806828.png)

2. Now try and run the `GetTenantUsers` API. It will take 20-40 seconds to complete first time you run it, because it has to do a bit of authentication, including getting a signed assertion from Key Vault. However, after the first run the token is cached and if you run the request again it should only take a second or so. 

> Note: Don't forget to enter both the `tenantId` of your Azure B2C Tenant (i.e., the one that was created as part of deploying the Identity Foundation) and the `x-api-key`. The API key can be found in Azure Portal in your Key Vault under Secrets menu with the name `RestApiKey`. 
>
> Hint: for debugging, you can disable the need of `x-api-key` by commenting out the middleware section in `Program.cs`, just be aware that this leaves your API unprotected:
>
> ```csharp
> app.UseMiddleware<ApiKeyMiddleware>();
> ```

![image-20230112001210631](assets/readme/image-20230112001210631.png)

## How  to Run In Production

<TO DO>
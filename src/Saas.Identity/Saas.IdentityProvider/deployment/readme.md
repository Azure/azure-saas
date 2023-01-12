# Deploying the Identity Provider and Permission Service

This deployment script provisions and configures the Azure services defining the SaaS Identify Foundation, which is back-bone of the Azure SaaS Dev Kit. 

## Before you begin

Before you begin, you should [fork](https://docs.github.com/en/get-started/quickstart/fork-a-repo) this GitHub repository to you own GitHub account - i.e., make it your own. A main purpose of the Azure SaaS Dev Kit is to boost your SaaS journey by providing a foundation for a solid start. It is however just a start and soon you will want to test variations, make you own changes, evolve the repo into something that is yours. By forking the repository, you will be ready to [commit](https://github.com/git-guides/git-commit) and check in your own code.

## Run the deployment script in a container, using docker (recommended)

> Tip: Although using container with Docker to run the script is the recommended approach, you'll find the steps to run the script without a container in a later section below. 

### Prerequisites 

The following steps assumes that your working on a Mac with a recent version of MacOS (Ventura or later recommended), on Linux or Windows 10/11.

Running on Windows 10/11 requires that [Windows Subsystem for Linux (WSL)](https://learn.microsoft.com/en-us/windows/wsl/install) have been enabled. WSL can be downloaded from the [Windows Store](https://www.microsoft.com/store/productId/9P9TQF7MRM4R). 

WSL lets you run a GNU/Linux environment (including [bash](https://www.gnu.org/software/bash/)) on Windows 10/11 without any modifications and without the need of a virtual machine.

We suggest using [Ubuntu 22.04](https://www.microsoft.com/store/productId/9PN20MSR04DW) on Windows, which can also also be downloaded from the Windows Store.

You should also install Azure Command Line Interface (az cli) from the terminal: [How to install the Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli).

### Why use a container?

There are two ways to run this deployment script. The recommended way to run the script is using a container that you build using [docker](https://docs.docker.com/get-docker/). 

This containerized approach will ensure that you have all the required dependencies installed and that you are running the script in a controlled environment. It will also minimize the chances that some other properties of your existing environment interferes with the script or that the script inadvertently interferes with your existing environment.

### Begin

To begin; open your GNU Linux terminal to the directory where you've [cloned](https://docs.github.com/en/repositories/creating-and-managing-repositories/cloning-a-repository) the [forked](https://docs.github.com/en/get-started/quickstart/fork-a-repo) version of ASDK. Should be something like:

````bash
/<project root>/azure-saas/src/Saas.Identity/Saas.IdentityProvider/deployment
````

![image-20230110094801956](assets/readme/image-20230110094801956.png)

> Tip: You can open the deployment project in Visual Code by typing `code .` in the terminal (Mac or Windows with WSL) from the directory.

### Building the script-run container

To run the script you must first build the container. To do this; run the following commands:

```bash
chmod +x build.sh # only needed the first time to set execute permissions on build.sh
./build.sh
```

This will take a few minutes. The container will be named `asdk-idprovider`.

### Running the deployment script using the container

When the container have been creates, run the script using the container with the following commands:

```bash 
chmod +x run.sh # only needed the first time to set execute permissions on run.sh
./run.sh
```

This will instantiate the container and mount the current root directory as a volume accessible from within the container. 

Mounting the root directory means that any edits to the `config.json` or anything any of the script files you see will immediately becomes effective without having to re-build the container. All you need to do is tun `./run.sh` again to make changes you made happen.

## Running the script

The first time you run the script, the script will automatically create a new instance of the `./config/config.json` (based on `./config/config-template.json`, after which the script will exit immediately with a request for additional information to be added to the configuration manifest in `config.json`. 

Specifically, the `initConfig` section must be filled out (see more details below):

```json
{
  "initConfig": {
    "userPrincipalId": "123e4567-e89b-12d3-a456-426652340000",
    "subscriptionId": "123e4567-e89b-12d3-a456-426652340000",
    "tenantId": "123e4567-e89b-12d3-a456-426652340000",
    "location": "enter the geo location, for instance 'westeurope'",
    "naming": {
      "solutionPrefix": "asdk",  // 'asdk' is the default prefix used
      "solutionName": "test" // leave as 'test' or change to some other name
    },
    "azureb2c": {
      "location": "Europe", // enter a valid Azure B2C region here. This is not the same as 'location'
      "countryCode": "DK", // enter a valid country code.
      "skuName": "PremiumP1", // can be Standard, PremiumP1 or PremiumP2.
      "tier": "A0" // leave this as 'A0'
    }
  },... // leave the remaining part of the configuration manifest unchanged.
```

### User Principal Id

Get the `userPrincipalId` by running following command, which will respond with a GUID:

```bash
az login # only do this if you're not logged in already
az ad signed-in-user show --query id
```

> The reason that the script doesn't pull the `userPrincipalId` automatically, is that some organization may require that this request can only be run from a *manage device*. Because the deployment script is run from inside a container this, can throw an error: "*AADSTS530003: Your device is required to be managed to access this resource.*"

### Azure Subscription Id

You may have multiple Azure subscriptions and thus manually choosing which subscription you want to use is the most practical approach for filling in the `subscriptionId`value. You can see your subscriptions in the [Azure Portal on the subscriptions page](https://ms.portal.azure.com/#view/Microsoft_Azure_Billing/SubscriptionsBlade), the value must be a GUID.

###  Tenant Id

Get the `tenantId`bu running the following command, which will respond with a GUID.

```bash
az login # only do this if you're not logged in already
az account show --query tenantId
```

> Note: The `id` that is returned by `az account show` is **not** the `userPrincipalId` mentioned above.

### Location

To get a list of valid locations run this command:

```bash
az account list-locations -o table
```

### The other values

The other values in `initConfig`:

| Value                  | Default   | Comment                                                      |
| ---------------------- | --------- | ------------------------------------------------------------ |
| `solutionPrefix`       | asdk      | The suggestion is to leave it as-is.                         |
| `solutionName`         | test      | The suggestion is to lead it as default or limit it to four letters. |
| `azureb2c/location`    | N/A       | Note that this is not the same as the location above, but is rather the names of the Azure AD regions available. Unfortunately, there's currently no command available for getting the list. |
| `azureb2c/countryCode` | N/A       | An available ISO country code                                |
| `azureb2c/skuName`     | PremiumP1 | Available options are `Standard`, `Premium1` and `Premium2`  |
| `azureb2c/tier`        | A0        | No known alternatives at the moment, please leave it as-is.  |

### Running the script

While running the script, you will be asked to log in twice. 

1. The first login is for your main Azure tenant.

   > Note: you're running the script outside of a container, you may already have been logged in, in which case the script will not ask you to log in again.

2. The second login is for logging into the Azure B2C Tenant that have just been created. This login is needed to make further changes to the Azure B2C tenant. 

### What if something goes wrong?

In most cases, if something goes wrong along the way, all you need to do is run the script again and it will skip the parts that have already been completed and re-try the parts that have not.

## Running deployment script on your computer w/o docker (not recommended)

You can also run the deployment script on you computing without using a container.

The script have been tested on:

-  Ubuntu 22.04 (will also run on WSL2 on Windows 10/11 with a Ubuntu 22.04 distro).
-  MacOS Ventura. 13.1+, including MacOS running on Apple Silicon.
   While not tested on other configurations, it will likely run recent Linux distros and versions as well as and earlier and recent versions of MacOS too.

Make sure that you have the following installed on your machine before running the script:

- [Az CLI v2.43.0+](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- [JQ v1.6+](https://linuxhint.com/bash_jq_command/) for Bash.
- Specifically on MacOS, you'll need a more recent of `bash` as the default version is rather old. For this you can install the latest version of bash using homebrew: [`brew install bash`](https://formulae.brew.sh/formula/bash).

When these requirements are met, the script can be run using the following command:

```bash
chmod +x start.sh
./start.sh
```

From there on everything else is virtually identical to running the script from inside a container.

## Now what?

The deployment script has run to it's completion and the Identity Framework have been deployed - providing that nothing went wrong, of course. It's time to kick the tires on The Azure SaaS Development Kit. ASDK is modular and the next step should be to run the Permission Service, as it's such a core component. 

We suggest that you run it locally first. This gives you an opportunity to attach a debugger to *see* and understand what's going on, so please head over to the Permission Service `ReadMe`, for more on locally as well as deploying it to Azure. 

The Permission Service is part of the repository that you *git cloned*, after you *git forked* it. You'll find it  here: `/<project root>/azure-saas/src/Saas.Identity/Saas.Permissions`.
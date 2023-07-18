# Frequently Asked Questions

## Scripting

### Why GNU Bash?

For scripting we looked at PowerShell and GNU Bash. We chose GNU Bash for it's versatility and ubiquity across platforms. With the advert of Windows Subsystem for Linux, the GNU Bash experience is as good on Windows as on Mac and Linux.

### Can I automate deployment using [ARM](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/overview) or even better [Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview?tabs=bicep)? 

Yes, and no. Bicep (ARM) is used when ever possible as part of the deployment script. The ASDK Identity Foundation relies on [Azure Active Directory B2C](https://learn.microsoft.com/en-us/azure/active-directory-b2c/overview) including [Azure Active Directory App Registrations](https://learn.microsoft.com/en-us/azure/active-directory/develop/active-directory-how-applications-are-added) and the provisioning of custom policies etc. At the time of writing those resources and their configurations cannot be fully automated by ARM and Bicep, unless augmented with embedded scripting, which leads to the next question.

### Could I leverage the  [Use deployment scripts in Bicep - Azure Resource Manager](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/deployment-script-bicep)  for performing the AD actions?

Yes, you could do this with some extra work. We've decided not to use it here for a couple of reasons:

First of all, the purpose of the Azure SaaS Dev Kit is not to provide a product or a service, but rather to provide a starting point and a reference that you can use to guide you on your SaaS journey to save you time and get you started on a path that is well-founded on experiences and learnings that others have done on their similar path.

Secondly, making changes and debugging scripts embedded into Bicep or ARM is time consuming. For a production ready system investing this time for establishing a solid CI/CD pipeline makes a lot of sense. However, while testing and exploring, keeping the scripting more accessible is the better choice, we believe. 

### Why do I have to use a container and Docker to run the script

Using a container will ensure that you have all the required dependencies in their correct configurations in a controlled environment. Using a container for deployment will also minimize the chances that some other properties of your existing environment might interfere with the script, or that the script inadvertently interferes with your existing environment.

Testing have also showed that the script in most instances runs faster when run from a container.

### Why do I have to build the container  myself, why not make it available in repository for me to pull?

We urge anyone using ASDK to *git fork* the ASDK repo and then *git clone* this forked version. In essence, the forked version becomes a snapshot of the current state of the ASDK and a starting point for your own customizable SaaS journey. Meanwhile, we expect to continue to evolve ASDK and do so without being limited by considerations of backwards compatibility. 

By everyone build their own scripting and deployment containers based on their current version or based on their individual changes and customizations, we believe that we've struck a good balance providing flexibility for usage and for the further evolution of ASDK. 

We plan to continue to monitor this decision as the ASDK matures and any reflections and input on this or anything else regarding ASDK is very welcome.

### Why am I required to install  tools locally when leveraging a container to run the script?

Besides Docker desktop, there's a requirement to install [Azure Command Line Tool](https://learn.microsoft.com/en-us/cli/azure/what-is-azure-cli) and [GitHub Command Line Tool](https://cli.github.com/). Those tools are also installed in the container. 

The short answer for these requirement is that these tools are foundational and we believe that most people working with GitHub and/or Azure would have them installed already.

The reason we need them locally, is because they are need to pass key variable and authentication data to the script when it runs. We decided to leverage those tools to minimize the number of times that you are asked to log on. 

In more technical terms; when the container runs, it runs as an immutable instance, however by passing environment variables to the container and leveraging volumes, we're able to persist some key data making the scripting experience faster and more seamless. This is especially helpful when/if you start making changes to the script and want to test these changes repeatedly.  

### What does it cost to run ASDK in my Azure Subscription?

The answer is *it depends*, on what you are doing with it, how much you do etc. However, as a test environment we've observed to spending to be approx. USD 30/Month when deployed in Western Europe during early 2023. 


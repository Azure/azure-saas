FROM mcr.microsoft.com/powershell:lts-ubuntu-20.04

RUN apt-get update && apt-get install curl -y
 
# Install the Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

#Install Required powershell modules
RUN pwsh -Command "Install-Module -Name Microsoft.Graph -Force"
RUN pwsh -Command "Install-Module -Name IefPolicies -Force"

WORKDIR /app


COPY ./Saas.IdentityProvider/scripts/B2C-Create.ps1 /app

COPY ./SaaS.Identity.IaC /app/Saas.Identity.IaC


ENTRYPOINT [ "pwsh", "./B2C-Create.ps1" ]
FROM mcr.microsoft.com/powershell:lts-ubuntu-20.04

RUN apt-get update && apt-get install curl -y
 
ENV DOCKER=true

# Install the Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

#Install Required powershell modules
RUN pwsh -Command "Install-Module -Name Microsoft.Graph -RequiredVersion 1.9.6 -Force -AllowClobber"
RUN pwsh -Command "Install-Module -Name Microsoft.Graph.Applications -RequiredVersion 1.9.6 -Force -AllowClobber" 
RUN pwsh -Command "Install-Module -Name Az.Accounts -RequiredVersion 2.8.0 -Force -AllowClobber" 

WORKDIR /app


COPY ./Saas.IdentityProvider /app/Saas.IdentityProvider

COPY ./SaaS.Identity.IaC /app/Saas.Identity.IaC


ENTRYPOINT [ "pwsh", "./Saas.IdentityProvider/scripts/B2C-Create.ps1" ]
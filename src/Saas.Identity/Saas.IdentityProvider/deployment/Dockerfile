FROM ubuntu:22.04

RUN apt-get update && apt-get install -y \
    curl \
    dnsutils \
    jq \
    sudo \
    libicu-dev \
    uuid-runtime \
    zip \
    && curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Install latest GitHub cli (gh)
# https://github.com/cli/cli/blob/trunk/docs/install_linux.md
RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && sudo chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && sudo apt update \
    && sudo apt install gh -y

# Install Azure Cli extension 'Storage-preview'
# RUN az extension add --name storage-preview
    
WORKDIR /asdk/Saas.IdentityProvider/deployment

ENV ASDK_ID_PROVIDER_DEPLOYMENT_BASE_DIR="/asdk/Saas.IdentityProvider/deployment"
FROM ubuntu:22.04

RUN apt-get update && apt-get install -y \
    curl \
    dnsutils \
    python3-pip \
    jq \
    sudo \
    libicu-dev \
    uuid-runtime \
    zip \
    dos2unix \
    python3-ruamel.yaml \
    && curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Install latest GitHub cli (gh)
# https://github.com/cli/cli/blob/trunk/docs/install_linux.md
RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | sudo dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && sudo chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | sudo tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && sudo apt update \
    && sudo apt install gh -y
    
RUN pip3 install ruamel.yaml

# Install Azure Cli extension 'Storage-preview'
# RUN az extension add --name storage-preview
    
WORKDIR /asdk

# trust repository in container
RUN git config --global --add safe.directory /asdk
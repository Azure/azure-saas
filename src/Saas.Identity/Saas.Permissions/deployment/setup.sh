#!/usr/bin/env bash

# shellcheck disable=SC1091
source "./constants.sh"

echo "Setting up the deployment environment."
echo "Settings execute permissions on necessary scripts files."

sudo chmod +x ./*.sh
sudo chmod +x ./script/*.sh
sudo chmod +x ./script/*.py

echo
echo "Setup complete. You can now run the deployment script using the command './run.sh'."
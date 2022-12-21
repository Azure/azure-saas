#!/bin/bash

source "./script/colors-module.sh"
source "./script/log-module.sh"

catch() {
  echo "catching!"
  if [ "$1" != "0" ]; then
    # error handling goes here
    echo "Error $1 occurred on $2"
  fi
}

function create-linux-user() {
    usr_name="$1"
    usr_path="$2"
    sub_path="$3"

    echo "Cleaning up before creating user ${usr_name}" \
        | log-output \
            --level info \
            --header "User: ${usr_name}"

    clean-up-linux-user "${usr_name}" "${usr_path}"

    echo "Creating user ${usr_name} with home directory ${usr_path} for storing certificates and keys." \
        | log-output \
            --level info

    # Creating a random 24 character password for the temperary 'b2c' shell user.
    usr_temp_pwd="$( cat /dev/urandom | tr -cd '[:graph:]' | head -c 24 )"

    # Encrypting the random passwords so that it can be used with the 'useradd' command when creating the 'b2c' user.
    encrypted_pwd="$( mkpasswd "${usr_temp_pwd}" --method=sha-512 )"

    # Creating the temp shell user providing the encrypted password.
    sudo useradd -m "${usr_name}" -p "${encrypted_pwd}" --home-dir "${usr_path}" > /dev/null

    if [[ -n "${sub_path}" ]]; then
        # Creating the sub directory for the user.
        sudo mkdir -p "${usr_path}/${sub_path}" > /dev/null
        # Changing the ownership of the sub directory to the temp user.
        sudo chown -R "${usr_name}:" "${usr_path}/${sub_path}" > /dev/null
    fi
}

function clean-up-linux-user() {
    usr_name="$1"
    homedir="$2"

    # Check if the user name is valid.
    if [[ -z "${usr_name}" ]]; then
        echo "User name '${usr_name}' is invalid. Unable to delete user" | log-out --level warning
        return
    fi

    # Check if the user already exist and delete it if it does.
    if id "${usr_name}" > /dev/null 2>&1 ; then
        echo "User ${usr_name} exist. Deleting it." \
            | log-output \
                --level info

        sudo deluser "${usr_name}" &> /dev/null \
            || echo "Failed to delete user ${usr_name}: $?" | log-output --level warning
    fi

    # Check if the group already exist and delete it if it does.
    if groups "${usr_name}" > /dev/null 2>&1 ; then
        echo "Group ${usr_name} exist. Deleting it." \
            | log-output \
                --level info

        sudo delgroup "${usr_name}" &> /dev/null \
            || echo "Failed to delete group ${usr_name}: $?" | log-output --level warning
    fi

    # Check if the home directory is valid.
    if ! [[ -d "${homedir}" \
        && ! "${homedir}" == "/home/" \
        && ! "${homedir}" == "/home"
        && ! "${homedir}" == "/" ]]; then

        echo "Home directory '${homedir}' is invalid." \
            | log-output \
                --level info
    fi

    # Check if the home directory already exist and delete it if it does.
    if [[ -d "${homedir}" ]] ; then \

        echo "Deleting ${homedir} and all files within it." \
            | log-output --level info

        sudo rm -r "${homedir}" &> /dev/null \
            || echo "Failed to delete home directory ${homedir}: $?" | log-output --level warning

    else
        echo "Home directory ${homedir} does not exist." \
            | log-output \
                --level info
    fi
}

function move-file-to-home-dir-of-user() {
    current_path="$1"
    destination_dir="$2"
    user="$3"

    filename="$( basename "${current_path}" )"
    destination_path="${destination_dir}/${filename}"

    sudo mv "${current_path}" "${destination_path}" \
        || echo "Failed to move file to ${destination_path}: $?" \
            | log-output \
                --level error \
                --header "Critical Error"

    sudo chown "${user}" "${destination_path}" \
        || echo "Failed to change ownership of file to ${user}: $?" \
            | log-output \
                --level error \
                --header "Critical Error"

    echo "${destination_path}"
    return
}
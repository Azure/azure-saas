#!/usr/bin/env bash

# just a little util to do a git archive of the current repo. Useful for backing up the local repo, just in case.
# change the target directory to your liking.

backup_target_dir="/mnt/d/Git-Backup"
archive_file_name="azure-saas-dev-kit"

archive_type="zip"
now=$( date +%Y%m%d%H%M%S )
backup_file="${backup_target_dir}/${archive_file_name}-${now}.${archive_type}"

repo_base="$( git rev-parse --show-toplevel )"
current_dir="$( pwd )"

cd "${repo_base}" || exit

echo "Backing up to ${backup_file}. Please wait..."
git archive --output="$backup_file" --format="${archive_type}" HEAD

cd "${current_dir}" || exit
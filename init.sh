#!/bin/bash

set -e

# Here, we are trying to save disk space in the project's home directory
# because there isn't enough for these packages.
#
# The env vars should be defined in .env to something in /tmp
#
# Related docs: https://docs.microsoft.com/en-us/nuget/Consume-Packages/managing-the-global-packages-and-cache-folders

echo "Found NUGET_PACKAGES to $NUGET_PACKAGES"
mkdir -p $NUGET_PACKAGES

echo "Found NUGET_HTTP_CACHE_PATH to $NUGET_HTTP_CACHE_PATH "
mkdir -p $NUGET_HTTP_CACHE_PATH
echo "Good luck"

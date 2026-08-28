#!/usr/bin/bash
set -x
dotnet new tool-manifest --force
pushd src
dotnet tool install trash
dotnet trash --version
popd
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    dos2unix dotnet-tools.json
fi

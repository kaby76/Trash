#!/usr/bin/bash
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    MSYS_NT*)   machine=Msys;;
    *)          machine="UNKNOWN:${unameOut}"
esac
CONFIG=Release
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    where=`pwd`/src/trash/bin/$CONFIG/
    where=`cygpath -d $where`
else
    where=`pwd`/src/trash/bin/$CONFIG/
fi
echo "$machine"
echo "$where"
echo dotnet nuget add source $where --name trtool-trash
set -e
dotnet nuget add source $where --name trtool-trash
dotnet nuget list source

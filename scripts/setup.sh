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
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
else
    cwd=`pwd`
fi
CONFIG=Release
echo "$machine"
echo "$cwd"
echo dotnet nuget add source $cwd/src/trash/bin/$CONFIG/ --name trtool-trash
dotnet nuget add source $cwd/src/trash/bin/$CONFIG/ --name trtool-trash > /dev/null 2>&1
dotnet nuget list source

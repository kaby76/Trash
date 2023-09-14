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
tools=`find . -name 'tr*.exe' | grep -v publish | awk -F '/' '{print $3}'`
echo $tools
for i in $tools
do
	dotnet nuget add source $cwd/src/$i/bin/$CONFIG/ --name trtool-$i > /dev/null 2>&1
done
dotnet nuget list source
